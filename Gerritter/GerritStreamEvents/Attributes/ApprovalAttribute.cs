
namespace Gerritter.GerritStreamEvents.Attributes
{
    public class ApprovalAttribute
    {
        public ApprovalAttribute(dynamic json)
        {
            type = GerritStream.GetValue(json, "type");
            description = GerritStream.GetValue(json, "description");
            value = int.Parse(GerritStream.GetValue(json, "value"));
            grantedOn = GerritStream.GetValue(json, "grantedOn");
            by = new AccountAttribute(GerritStream.GetValue(json, "by"));
        }

        public string type;
        public string description;
        public int value;
        public string grantedOn;
        public AccountAttribute by;
    }
}
