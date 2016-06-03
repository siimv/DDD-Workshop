using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    public class CookFood : MessageBase
    {
        public JObject Order { get; set; }
    }
}