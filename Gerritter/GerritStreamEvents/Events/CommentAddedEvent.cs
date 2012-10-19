using Gerritter.GerritStreamEvents.Attributes;

namespace Gerritter.GerritStreamEvents.Events
{
    public class CommentAddedEvent : IGerritEvent
    {
        public CommentAddedEvent(dynamic json)
        {
            type = GerritStream.GetValue(json, "type");
            change = new ChangeAttribute(GerritStream.GetValue(json, "change"));
            patchset = new PatchsetAttribute(GerritStream.GetValue(json, "patchSet"));
            author = new AccountAttribute(GerritStream.GetValue(json, "author"));
            comment = GerritStream.GetValue(json, "comment");

            approvals = new ApprovalsAttribute(GerritStream.GetValue(json, "approvals"));
        }

        private string type;

        private ChangeAttribute change;
        private PatchsetAttribute patchset;
        private AccountAttribute author;
        private string comment;

        public ApprovalsAttribute approvals;

        public EventType EventType { get { return Event.GetEventTyepe(type); } }
        public string ProjectName { get { return change.project; } }

        public NotificationMessage CreateNotificationMessage()
        {
            string notifyTitle = change.project + " " + "Comment Added";
            string notifyMessage = "";
            notifyMessage += change.ToString();
            notifyMessage += patchset.ToString();
            notifyMessage += author.FormatWithLabel("author");
            foreach (var approval in approvals)
            {
                notifyMessage += string.Format("'{0}':'{1}' ", approval.description, approval.value);
            }
            notifyMessage += "\n";
            if (comment != null && comment != "")
            {
                notifyMessage += string.Format("comment:\n'{0}'", comment);
            }

            string url = change.url;

            return new NotificationMessage(notifyTitle, notifyMessage, url);
        }
    }
}
