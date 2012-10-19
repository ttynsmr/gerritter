using Gerritter.GerritStreamEvents.Attributes;

namespace Gerritter.GerritStreamEvents.Events
{
    public class ChangeMergedEvent : IGerritEvent
    {
        public ChangeMergedEvent(dynamic json)
        {
            type = GerritStream.GetValue(json, "type");
            change = new ChangeAttribute(GerritStream.GetValue(json, "change"));
            patchset = new PatchsetAttribute(GerritStream.GetValue(json, "patchSet"));
            submitter = new AccountAttribute(GerritStream.GetValue(json, "submitter"));
        }

        private string type;

        private ChangeAttribute change;
        private PatchsetAttribute patchset;
        private AccountAttribute submitter;

        public EventType EventType { get { return Event.GetEventTyepe(type); } }
        public string ProjectName { get { return change.project; } }

        public NotificationMessage CreateNotificationMessage()
        {
            string notifyTitle = change.project + " " + "Change Merged";
            string notifyMessage = "";
            notifyMessage += change.ToString();
            notifyMessage += submitter.FormatWithLabel("submitter");

            string url = change.url;

            return new NotificationMessage(notifyTitle, notifyMessage, url);
        }
    }
}
