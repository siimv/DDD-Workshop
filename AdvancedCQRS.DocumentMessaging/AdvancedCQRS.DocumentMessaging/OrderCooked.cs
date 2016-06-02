using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    public class OrderCooked : MessageBase
    {
        public JObject Order { get; set; }
    }
}