using System;

namespace AdvancedCQRS.DocumentMessaging
{
    public class PrintingOrderHandler : IHandleOrder<OrderPaid>, IHandleOrder<OrderPlaced>
    {
        public void Handle(OrderPaid order)
        {
            Console.WriteLine($"Order paid: \n\r {order.Order}");
        }

        public void Handle(OrderPlaced order)
        {
            Console.WriteLine($"Order placed: \n\r {order.Order}");
        }
    }
}