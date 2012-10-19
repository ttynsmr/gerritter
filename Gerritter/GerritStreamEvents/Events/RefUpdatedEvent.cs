using Gerritter.GerritStreamEvents.Attributes;

namespace Gerritter.GerritStreamEvents.Events
{
    public class RefUpdatedEvent : IGerritEvent
    {
        public RefUpdatedEvent(dynamic json)
        {
            type = GerritStream.GetValue(json, "type");
            submitter = new AccountAttribute(GerritStream.GetValue(json, "submitter"));
            refUpdate = new RefupdateAttribute(GerritStream.GetValue(json, "refUpdate"));
        }

        private string type;

        private AccountAttribute submitter;
        private RefupdateAttribute refUpdate;

        public EventType EventType { get { return Event.GetEventTyepe(type); } }
        public string ProjectName { get { return refUpdate.project; } }

        public NotificationMessage CreateNotificationMessage()
        {
            string notifyTitle = refUpdate.project + " " + "Ref Updated";
            string notifyMessage = "";
            notifyMessage += string.Format("refName:'{0}'\n", refUpdate.refName);
            notifyMessage += string.Format("newRev:'{0}'\n", refUpdate.newRev);
            notifyMessage += string.Format("oldRev:'{0}'\n", refUpdate.oldRev);
            notifyMessage += submitter.FormatWithLabel("submitter");

            return new NotificationMessage(notifyTitle, notifyMessage, "");
        }
    }
}
