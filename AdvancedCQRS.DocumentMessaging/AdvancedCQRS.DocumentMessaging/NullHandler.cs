using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    public class NullHandler : IHandleOrder
    {
        public void Handle(JObject order)
        {
        }
    }
}