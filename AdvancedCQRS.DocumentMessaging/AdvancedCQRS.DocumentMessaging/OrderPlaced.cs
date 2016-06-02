using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    public class OrderPlaced : MessageBase
    {
        public JObject Order { get; set; }
    }
}