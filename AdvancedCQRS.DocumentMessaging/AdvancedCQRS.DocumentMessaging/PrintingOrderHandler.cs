using System;

namespace AdvancedCQRS.DocumentMessaging
{
    public class PrintingOrderHandler : IHandleOrder<IMessage>, IHandleOrder<OrderPaid>
    {
        public void Handle(OrderPaid order)
        {
            WriteAction(order);
            Console.WriteLine($"Order paid: \n\r {order.Order}");
        }

        public void Handle(IMessage order)
        {
            WriteAction(order);
        }

        private static void WriteAction(IMessage order)
        {
            Console.WriteLine($"{order.GetType().Name}: ${order.CorrelationId}");
        }
    }
}