using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    public class HandlerMultiplexer : IHandleOrder
    {
        private readonly IHandleOrder[] _handlers;

        public HandlerMultiplexer(params IHandleOrder[] handlers)
        {
            _handlers = handlers;
        }

        public void Handle(JObject order)
        {
            foreach (var handler in _handlers)
            {
                handler.Handle(order);
            }
        }
    }
}