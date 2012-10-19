
namespace Gerritter.GerritStreamEvents.Attributes
{
    public class RefupdateAttribute
    {
        public RefupdateAttribute(dynamic json)
        {
            oldRev = GerritStream.GetValue(json, "oldRev");
            newRev = GerritStream.GetValue(json, "newRev");
            project = GerritStream.GetValue(json, "project");
            refName = GerritStream.GetValue(json, "refName");
        }

        public string oldRev;
        public string newRev;
        public string project;
        public string refName;
    }
}
