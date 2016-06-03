using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    public class PriceOrder : MessageBase
    {
        public JObject Order { get; set; }
    }
}