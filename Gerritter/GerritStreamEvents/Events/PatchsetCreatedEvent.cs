using Gerritter.GerritStreamEvents.Attributes;

namespace Gerritter.GerritStreamEvents.Events
{
    public class PatchsetCreatedEvent : IGerritEvent
    {
        public PatchsetCreatedEvent(dynamic json)
        {
            type = GerritStream.GetValue(json, "type");
            change = new ChangeAttribute(GerritStream.GetValue(json, "change"));
            patchset = new PatchsetAttribute(GerritStream.GetValue(json, "patchSet"));
            uploader = new AccountAttribute(GerritStream.GetValue(json, "uploader"));
        }

        private string type;

        private ChangeAttribute change;
        private PatchsetAttribute patchset;
        private AccountAttribute uploader;

        public EventType EventType { get { return Event.GetEventTyepe(type); } }
        public string ProjectName { get { return change.project; } }

        public NotificationMessage CreateNotificationMessage()
        {
            string notifyTitle = change.project + " " + "Patchset Created";

            string notifyMessage = "";
            notifyMessage += change.ToString();
            notifyMessage += patchset.ToString();

            string url = change.url;

            return new NotificationMessage(notifyTitle, notifyMessage, url);
        }
    }
}
