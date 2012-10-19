
namespace Gerritter.GerritStreamEvents.Attributes
{
    public class AccountAttribute
    {
        public AccountAttribute(dynamic json)
        {
            name = GerritStream.GetValue(json, "name");
            email = GerritStream.GetValue(json, "email");
        }

        public string name;
        public string email;

        public override string ToString()
        {
            if (email != null && email != "")
            {
                return string.Format("{0} <{1}>", name, email);
            }
            else
            {
                return string.Format("{0}", name);
            }
        }

        public string FormatWithLabel(string label)
        {
            return string.Format("{0}:'{1}'\n", label, ToString());
        }
    }
}
