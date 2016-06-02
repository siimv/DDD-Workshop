using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    public class RoundRobinDispatcher : IHandleOrder
    {
        private readonly Queue<IHandleOrder> _handlers;

        public RoundRobinDispatcher(params IHandleOrder[] handlers)
        {
            _handlers = new Queue<IHandleOrder>(handlers);
        }

        public void Handle(JObject order)
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