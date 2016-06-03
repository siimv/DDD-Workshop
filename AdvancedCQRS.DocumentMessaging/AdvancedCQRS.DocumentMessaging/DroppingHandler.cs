using System;

namespace AdvancedCQRS.DocumentMessaging
{
    public static class DroppingHandler
    {
        public static IHandleOrder<T> WrapWithDropper<T>(this IHandleOrder<T> handler) where T : IMessage
        {
            return new DroppingHandler<T>(handler);
        }
    }

    public class DroppingHandler<T> : IHandleOrder<T> where T : IMessage
    {
        private readonly IHandleOrder<T> _handler;
        private Random _random = new Random();

        public DroppingHandler(IHandleOrder<T> handler)
        {
            _handler = handler;
        }

        public void Handle(T order)
        {
            if (_random.NextDouble() < 0.15)
            {
                //Drop order
                //Console.WriteLine($"Dropped order: {order.CorrelationId}");
            }
            else
            {
                _handler.Handle(order);
            }
        }
    }
}