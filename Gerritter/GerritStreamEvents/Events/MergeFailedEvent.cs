using Gerritter.GerritStreamEvents.Attributes;

namespace Gerritter.GerritStreamEvents.Events
{
    public class MergeFailedEvent : IGerritEvent
    {
        public MergeFailedEvent(dynamic json)
        {
            type = GerritStream.GetValue(json, "type");
            change = new ChangeAttribute(GerritStream.GetValue(json, "change"));
            patchset = new PatchsetAttribute(GerritStream.GetValue(json, "patchSet"));
            submitter = new AccountAttribute(GerritStream.GetValue(json, "submitter"));
            reason = GerritStream.GetValue(json, "reason");
        }

        private string type;

        private ChangeAttribute change;
        private PatchsetAttribute patchset;
        private AccountAttribute submitter;
        private string reason;

        public EventType EventType { get { return Event.GetEventTyepe(type); } }
        public string ProjectName { get { return change.project; } }

        public NotificationMessage CreateNotificationMessage()
        {
            string notifyTitle = change.project + " " + "Merge Failed";
            string notifyMessage = "";
            notifyMessage += change.ToString();
            notifyMessage += submitter.FormatWithLabel("submitter");
            notifyMessage += "\n";
            if (reason != null && reason != "")
            {
                notifyMessage += string.Format("reason:\n'{0}'", reason);
            }

            string url = change.url;

            return new NotificationMessage(notifyTitle, notifyMessage, url);
        }
    }
}
