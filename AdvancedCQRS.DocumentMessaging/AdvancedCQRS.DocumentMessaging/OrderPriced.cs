using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    public class OrderPriced : MessageBase
    {
        public JObject Order { get; set; }
    }
}