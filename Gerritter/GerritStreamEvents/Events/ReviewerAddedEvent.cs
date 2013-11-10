using Gerritter.GerritStreamEvents.Attributes;

namespace Gerritter.GerritStreamEvents.Events
{
    public class ReviewerAddedEvent : IGerritEvent
    {
        public ReviewerAddedEvent(dynamic json)
        {
            type = GerritStream.GetValue(json, "type");
            change = new ChangeAttribute(GerritStream.GetValue(json, "change"));
            patchset = new PatchsetAttribute(GerritStream.GetValue(json, "patchSet"));
            reviewer = new AccountAttribute(GerritStream.GetValue(json, "reviewer"));
        }

        private string type;

        private ChangeAttribute change;
        private PatchsetAttribute patchset;
        private AccountAttribute reviewer;

        public EventType EventType { get { return Event.GetEventTyepe(type); } }
        public string ProjectName { get { return change.project; } }

        public NotificationMessage CreateNotificationMessage()
        {
            string notifyTitle = change.project + " " + "Reviewer Added";
            string notifyMessage = "";
            notifyMessage += change.ToString();
            notifyMessage += reviewer.FormatWithLabel("reviewer");

            string url = change.url;

            return new NotificationMessage(notifyTitle, notifyMessage, url);
        }
    }
}
