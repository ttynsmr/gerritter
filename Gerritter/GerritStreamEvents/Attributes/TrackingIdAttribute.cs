
namespace Gerritter.GerritStreamEvents.Attributes
{
    public class TrackingIdAttribute
    {
        public TrackingIdAttribute(dynamic json)
        {
            system = GerritStream.GetValue(json, "system");
            id = GerritStream.GetValue(json, "id");
        }

        public string system;
        public string id;
    }
}
