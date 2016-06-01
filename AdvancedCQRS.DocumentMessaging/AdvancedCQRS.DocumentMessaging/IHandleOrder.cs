using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    public interface IHandleOrder
    {
        void Handle(JObject order);
    }
}