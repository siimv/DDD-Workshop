using System;

namespace AdvancedCQRS.DocumentMessaging
{
    public class PrintingOrderHandler : IHandleOrder<OrderPaid>, IHandleOrder<IMessage>
    {
        public void Handle(OrderPaid order)
        {
            Console.WriteLine($"Order paid: \n\r {order.Order}");
        }

        public void Handle(IMessage order)
        {
            Console.WriteLine($"{order.GetType().Name}: ${order.CorrelationId}");
        }
    }
}