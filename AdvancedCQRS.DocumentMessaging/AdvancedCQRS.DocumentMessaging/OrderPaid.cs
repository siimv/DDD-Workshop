using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    public class OrderPaid : MessageBase
    {
        public JObject Order { get; set; }
    }
}