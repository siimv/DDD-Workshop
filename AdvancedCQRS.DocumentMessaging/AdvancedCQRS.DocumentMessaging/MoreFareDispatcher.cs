using System.Linq;
using System.Threading;

namespace AdvancedCQRS.DocumentMessaging
{
    public class MoreFareDispatcher
    {
        public static MoreFareDispatcher<T> Create<T>(params QueuedHandler<T>[] handlers) where T : IMessage
        {
            return new MoreFareDispatcher<T>(handlers);
        }
    }

    public class MoreFareDispatcher<T> : IHandleOrder<T> where T : IMessage
    {
        private readonly QueuedHandler<T>[] _handlers;

        public MoreFareDispatcher(params QueuedHandler<T>[] handlers)
        {
            _handlers = handlers;
        }

        public void Handle(T order)
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