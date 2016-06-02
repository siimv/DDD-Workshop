using System.Linq;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    public class MoreFareDispatcher : IHandleOrder
    {
        private readonly QueuedHandler[] _handlers;

        public MoreFareDispatcher(params QueuedHandler[] handlers)
        {
            _handlers = handlers;
        }

        public void Handle(JObject order)
        {
            while (true)
            {
                var handler = _handlers.FirstOrDefault(x => x.NumberOfMessages < 5);
                if (handler != null)
                {
                    handler.Handle(order);
                    return;
                }
                
                Thread.Sleep(1);
            }
        }
    }
}