using Gerritter.GerritStreamEvents.Attributes;

namespace Gerritter.GerritStreamEvents.Events
{
    public class ChangeAbandonedEvent : IGerritEvent
    {
        public ChangeAbandonedEvent(dynamic json)
        {
            type = GerritStream.GetValue(json, "type");
            change = new ChangeAttribute(GerritStream.GetValue(json, "change"));
            patchset = new PatchsetAttribute(GerritStream.GetValue(json, "patchSet"));
            abandoner = new AccountAttribute(GerritStream.GetValue(json, "abandoner"));
            reason = GerritStream.GetValue(json, "reason");
        }

        private string type;

        private ChangeAttribute change;
        private PatchsetAttribute patchset;
        private AccountAttribute abandoner;
        private string reason;

        public EventType EventType { get { return Event.GetEventTyepe(type); } }
        public string ProjectName { get { return change.project; } }

        public NotificationMessage CreateNotificationMessage()
        {
            string notifyTitle = change.project + " " + "Change Abandoned";
            string notifyMessage = "";
            notifyMessage += change.ToString();
            notifyMessage += patchset.ToString();
            notifyMessage += abandoner.FormatWithLabel("abandoner");
            notifyMessage += string.Format("reason:'{0}'", reason);

            string url = change.url;

            return new NotificationMessage(notifyTitle, notifyMessage, url);
        }
    }
}
