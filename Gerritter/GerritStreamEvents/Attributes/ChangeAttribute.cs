
namespace Gerritter.GerritStreamEvents.Attributes
{
    public class ChangeAttribute
    {
        public ChangeAttribute(dynamic json)
        {
            project = GerritStream.GetValue(json, "project");
            branch = GerritStream.GetValue(json, "branch");
            topic = GerritStream.GetValue(json, "topic");
            id = GerritStream.GetValue(json, "id");
            number = int.Parse(GerritStream.GetValue(json, "number"));
            subject = GerritStream.GetValue(json, "subject");
            owner = new AccountAttribute(GerritStream.GetValue(json, "owner"));
            url = GerritStream.GetValue(json, "url");
            lastupdated = GerritStream.GetValue(json, "lastupdated");
            sortkey = GerritStream.GetValue(json, "sortkey");
            open = GerritStream.GetValue(json, "open");
            trackingids = new TrackingIdAttribute(GerritStream.GetValue(json, "trackingids"));
            currentpatchset = new PatchsetAttribute(GerritStream.GetValue(json, "currentpatchset"));
            patchsets = new PatchsetAttribute(GerritStream.GetValue(json, "patchsets"));
        }

        public string project;
        public string branch;
        public string topic;
        public string id;
        public int number;
        public string subject;
        public AccountAttribute owner;
        public string url;
        public string lastupdated;
        public string sortkey;
        public string open;
        public TrackingIdAttribute trackingids;
        public PatchsetAttribute currentpatchset;
        public PatchsetAttribute patchsets;

        public override string ToString()
        {
            string notifyMessage = "";
            notifyMessage += string.Format("project:'{0}'\n", project);
            notifyMessage += string.Format("branch:'{0}'\n", branch);
            if (topic != null && topic != "")
            {
                notifyMessage += string.Format("topic:'{0}'\n", topic);
            }
            notifyMessage += owner.FormatWithLabel("owner");
            notifyMessage += string.Format("subject:'{0}'\n", subject);
            if (url != null && url != "")
            {
                notifyMessage += string.Format("url:'{0}'\n", url);
            }

            return notifyMessage;
        }
    }
}
