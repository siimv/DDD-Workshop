namespace AdvancedCQRS.DocumentMessaging
{
    public class NullHandler : IHandleOrder<OrderPaid>
    {
        public void Handle(OrderPaid order)
        {
        }
    }
}