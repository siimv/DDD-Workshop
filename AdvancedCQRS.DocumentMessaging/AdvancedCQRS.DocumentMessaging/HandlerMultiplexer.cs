namespace AdvancedCQRS.DocumentMessaging
{
    public class HandlerMultiplexer<T> : IHandleOrder<T> where T : IMessage
    {
        private readonly IHandleOrder<T>[] _handlers;

        public HandlerMultiplexer(params IHandleOrder<T>[] handlers)
        {
            _handlers = handlers;
        }

        public void Handle(T order)
        {
            foreach (var handler in _handlers)
            {
                handler.Handle(order);
            }
        }
    }
}