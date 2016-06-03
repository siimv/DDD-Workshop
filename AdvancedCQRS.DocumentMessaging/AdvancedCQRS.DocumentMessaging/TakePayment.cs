using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    public class TakePayment : MessageBase
    {
        public JObject Order { get; set; }
    }
}