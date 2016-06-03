using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    public class RetryCooking : MessageBase
    {
        public JObject Order { get; set; }
    }
}