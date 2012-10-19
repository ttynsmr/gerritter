
namespace Gerritter.GerritStreamEvents.Attributes
{
    public class PatchsetAttribute
    {
        public PatchsetAttribute(dynamic json)
        {
            number = GerritStream.GetValue(json, "number");
            revision = GerritStream.GetValue(json, "revision");
            reference = GerritStream.GetValue(json, "ref");
            uploader = new AccountAttribute(GerritStream.GetValue(json, "uploader"));
            approvals = new ApprovalsAttribute(GerritStream.GetValue(json, "approvals"));
        }

        public string number;
        public string revision;
        public string reference;//ref
        public AccountAttribute uploader;
        public ApprovalsAttribute approvals;

        public override string ToString()
        {
            string notifyMessage = "";
            notifyMessage += string.Format("patchset number:'{0}'\n", number);
            notifyMessage += uploader.FormatWithLabel("uploader");

            return notifyMessage;
        }
    }
}
