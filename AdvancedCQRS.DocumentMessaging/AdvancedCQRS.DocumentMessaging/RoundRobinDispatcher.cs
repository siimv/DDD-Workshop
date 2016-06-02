using System.Collections.Generic;

namespace AdvancedCQRS.DocumentMessaging
{
    public class RoundRobinDispatcher<T> : IHandleOrder<T> where T : IMessage
    {
        private readonly Queue<IHandleOrder<T>> _handlers;

        public RoundRobinDispatcher(params IHandleOrder<T>[] handlers)
        {
            _handlers = new Queue<IHandleOrder<T>>(handlers);
        }

        public void Handle(T order)
        {
            var handler = _handlers.Dequeue();

            try
            {
                handler.Handle(order);
            }
            finally
            {
                _handlers.Enqueue(handler);
            }
        }
    }
}