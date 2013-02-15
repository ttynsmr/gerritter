using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Codeplex.Data;
using Gerritter.GerritStreamEvents;
using Gerritter.GerritStreamEvents.Events;
using Growl.Connector;

namespace Gerritter
{
    public partial class MainForm : Form
    {
        private NotificationMessage ConvertJsonToNotificationMessage(string jsonString)
        {
            if (jsonString == null)
            {
                return null;
            }

            var gerritEvent = ParseEventJson(jsonString);

            if (IsNotifiable(gerritEvent) == false)
            {
                return null;
            }

            return gerritEvent.CreateNotificationMessage();
        }

        private static IGerritEvent ParseEventJson(string jsonString)
        {
            IGerritEvent gerritEvent = null;
            var json = DynamicJson.Parse(jsonString);
            switch (Event.GetEventTyepe(json.type as string))
            {
                case EventType.PatchsetCreated:
                    gerritEvent = new PatchsetCreatedEvent(json);
                    break;

                case EventType.ChangeAbandoned:
                    gerritEvent = new ChangeAbandonedEvent(json);
                    break;

                case EventType.ChangeRestored:
                    gerritEvent = new ChangeRestoredEvent(json);
                    break;

                case EventType.ChangeMerged:
                    gerritEvent = new ChangeMergedEvent(json);
                    break;

                case EventType.CommentAdded:
                    gerritEvent = new CommentAddedEvent(json);
                    break;

                case EventType.RefUpdated:
                    gerritEvent = new RefUpdatedEvent(json);
                    break;
            }
            return gerritEvent;
        }

        private void ShowMessage(NotificationMessage notificationMessage)
        {
            NotifyToGrowl(notificationMessage);
        }

        private bool IsNotifiable(IGerritEvent gerritEvent)
        {
            var checkedProjects = projectListBox.CheckedItems.Cast<Project>();
            if (!checkedProjects.Any(project => { return project.name == gerritEvent.ProjectName; }))
            {
                return false;
            }

            var checkedEvents = eventListBox.CheckedItems.Cast<EventType>();
            if (!checkedEvents.Any(eventType => { return eventType == gerritEvent.EventType; }))
            {
                return false;
            }

            return true;
        }

        #region SSH connecter
        private Process process;

        private void ConnectGerritStreamEvents()
        {
            exitRequested = false;
            stopRequested = false;

            startButton.Enabled = false;
            stopButton.Enabled = true;

            cygwinBinDirectory.ReadOnly = true;
            gerritHost.ReadOnly = true;
            gerritPort.ReadOnly = true;

            string directoryOfCygwin = cygwinBinDirectory.Text;
            string ssh = directoryOfCygwin + @"ssh.exe";
            string args = string.Format(@"-t -t -p {0} {1} gerrit stream-events", gerritPort.Text, gerritHost.Text);

            var startInfo = new ProcessStartInfo(ssh, args);
            startInfo.RedirectStandardInput = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            //startInfo.WorkingDirectory = workingDirectory;
            startInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;
            startInfo.CreateNoWindow = true;

            process = new Process();
            process.StartInfo = startInfo;
            process.OutputDataReceived += new DataReceivedEventHandler((object sender, DataReceivedEventArgs e) =>
            {
                var message = ConvertJsonToNotificationMessage(e.Data);
                if (message != null)
                {
                    ShowMessage(message);
                }
            });
            process.ErrorDataReceived += new DataReceivedEventHandler((object sender, DataReceivedEventArgs e) =>
            {
                Debug.WriteLine("error:" + e.Data);
            });
            process.Exited += new EventHandler((object sender, EventArgs e) =>
            {
                DisconnectGerritStreamEvents();

                if (exitRequested)
                {
                    return;
                }

                if (stopRequested)
                {
                    EnableConnectButton();
                }
                else
                {
                    if (autoReconnect)
                    {
                        System.Threading.Thread.Sleep(5000);
                        ConnectGerritStreamEvents();
                        return;
                    }

                    var result = MessageBox.Show("Connection lost.\nDo you want to reconnect now?", "Gerritter (" + gerritHost.Text + ":" + gerritPort.Text + ")", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                    switch (result)
                    {
                        case System.Windows.Forms.DialogResult.Yes:
                            ConnectGerritStreamEvents();
                            break;
                        case System.Windows.Forms.DialogResult.No:
                            EnableConnectButton();
                            break;
                    }
                }
            });
            process.EnableRaisingEvents = true;
            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            ShowMessage(new NotificationMessage("Connected", "Event notification started.", null));
            SetTrayMessage(gerritHost.Text + ":" + gerritPort.Text);
        }

        delegate void EnableConnectButtonDelegate();

        private void EnableConnectButton()
        {
            if (InvokeRequired)
            {
                Invoke(new EnableConnectButtonDelegate(EnableConnectButton));
            }
            else
            {
                startButton.Enabled = true;
                stopButton.Enabled = false;

                cygwinBinDirectory.ReadOnly = false;
                gerritHost.ReadOnly = false;
                gerritPort.ReadOnly = false;
            }
        }

        private void DisconnectGerritStreamEvents()
        {
            if (process == null)
            {
                return;
            }

            if (process.HasExited)
            {
                return;
            }

            process.Kill();

            process = null;
            ShowMessage(new NotificationMessage("Disconnected", "Event notification stopped.", null));
            SetTrayMessage("");
        }
        #endregion

        #region Application Settings
        private void SaveSettings()
        {
            Properties.Settings.Default.CygwinBinDirectory = cygwinBinDirectory.Text;
            Properties.Settings.Default.GerritHostName = gerritHost.Text;
            Properties.Settings.Default.GerritPort = gerritPort.Text;
            Properties.Settings.Default.Save();
        }

        private void LoadSettings()
        {
            cygwinBinDirectory.Text = Properties.Settings.Default.CygwinBinDirectory;
            gerritHost.Text = Properties.Settings.Default.GerritHostName;
            gerritPort.Text = Properties.Settings.Default.GerritPort;
        }
        #endregion

        #region Form stuff
        public MainForm()
        {
            InitializeComponent();
        }

        private void SetTrayMessage(string message)
        {
            if (message.Length > 0)
            {
                notifyIcon.Text = this.Text + " (" + message + ")";
            }
            else
            {
                notifyIcon.Text = this.Text;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            notifyIcon.Icon = this.Icon;
            SetTrayMessage("");

            LoadSettings();

            InitializeGrowlConnector();

            InitializeEvents();

            ShowMessage(new NotificationMessage("Hello", "Click \"Start\" to start event notification.", null));
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            DisconnectGerritStreamEvents();
            ShowMessage(new NotificationMessage("Bye", "See you again.", null));
        }


        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!exitRequested)
            {
                this.ShowInTaskbar = false;
                this.Hide();
                e.Cancel = true;
            }
        }
        #endregion

        #region Notify icon events
        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            this.ShowInTaskbar = true;
            this.Show();
        }
        #endregion

        private void StartButton_Click(object sender, EventArgs e)
        {
            RefreshProjects();
            ConnectGerritStreamEvents();
        }

        private bool exitRequested = false;
        private void QuitButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
            exitRequested = true;
            this.Close();
        }


        #region Growl Connector
        private GrowlConnector growl;
        private NotificationType notificationType;
        private Growl.Connector.Application application;

        private void InitializeGrowlConnector()
        {
            growl = new GrowlConnector();
            application = new Growl.Connector.Application("Gerritter");
            notificationType = new NotificationType("Gerritter notify", "Gerritter Notification");
            growl.Register(application, new NotificationType[] { notificationType });

            growl.NotificationCallback += new GrowlConnector.CallbackEventHandler(growl_NotificationCallback);
        }

        void growl_NotificationCallback(Response response, CallbackData callbackContext, object state)
        {
            switch (callbackContext.Result)
            {
                case Growl.CoreLibrary.CallbackResult.CLICK:
                    if (callbackContext.Data == "OPEN_URL")
                    {
                        Notification notification = state as Notification;

                        string url = notification.CustomTextAttributes["url"];
                        if (url != null && url.Substring(0, 4) == "http")
                        {
                            Process.Start(url);
                        }
                    }
                    break;

                case Growl.CoreLibrary.CallbackResult.CLOSE:
                    break;
                case Growl.CoreLibrary.CallbackResult.TIMEDOUT:
                    break;
            }
        }

        private void NotifyToGrowl(NotificationMessage notificationMessage)
        {
            if (notificationMessage.Title == "" || notificationMessage.Message == "")
            {
                return;
            }

            var notification = new Notification(application.Name, notificationType.Name, DateTime.Now.Ticks.ToString(), notificationMessage.Title, notificationMessage.Message);
            notification.Icon = this.Icon.ToBitmap();
            if (notificationMessage.Url != null)
            {
                notification.CustomTextAttributes.Add("url", notificationMessage.Url);
            }
            growl.Notify(notification, new CallbackContext("OPEN_URL", "string"), notification);
        }
        #endregion

        private void InitializeEvents()
        {
            var events = Enum.GetNames(typeof(EventType));
            foreach (var eventTypeName in events)
            {
                EventType eventType;
                if (Enum.TryParse<EventType>(eventTypeName, out eventType))
                {
                    var index = eventListBox.Items.Add(eventType);
                    eventListBox.SetItemChecked(index, true);
                }
            }
        }

        public class Project
        {
            public Project(string name)
            {
                this.name = name;
            }

            public string name;

            public override string ToString()
            {
                return name;
            }
        }

        private void RefreshProjects()
        {
            var resultProjects = RunGerritLsProjects();
            var projectNames = new List<string>(resultProjects.Split('\n'));
            projectNames.RemoveAll(projectName => { return projectName == ""; });

            projectListBox.Items.Clear();
            projectNames.ForEach(projectName => { projectListBox.Items.Add(new Project(projectName), true); });
        }

        private string RunGerritLsProjects()
        {
            string directoryOfCygwin = cygwinBinDirectory.Text;
            string ssh = directoryOfCygwin + @"ssh.exe";
            string args = string.Format(@"-t -t -p {0} {1} gerrit ls-projects", gerritPort.Text, gerritHost.Text);

            var startInfo = new ProcessStartInfo(ssh, args);
            startInfo.RedirectStandardInput = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.UseShellExecute = false;
            //startInfo.WorkingDirectory = workingDirectory;
            startInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;
            startInfo.CreateNoWindow = true;

            var process = Process.Start(startInfo);
            process.WaitForExit();

            var results = process.StandardOutput.ReadToEnd();
            return results;
        }

        private bool autoReconnect = false;

        private void checkBoxAutoReconnect_CheckedChanged(object sender, EventArgs e)
        {
            autoReconnect = (sender as CheckBox).Checked;
        }

        private bool stopRequested = false;
        private void StopButton_Click(object sender, EventArgs e)
        {
            stopRequested = true;
            process.Kill();
        }
    }
}
