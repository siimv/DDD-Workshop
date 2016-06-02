using System;

namespace AdvancedCQRS.DocumentMessaging
{
    public class PrintingOrderHandler : IHandleOrder<OrderPaid>
    {
        public void Handle(OrderPaid order)
        {
            Console.WriteLine(order.Order);
        }
    }
}