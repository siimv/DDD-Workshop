namespace AdvancedCQRS.DocumentMessaging
{
    public interface IMidget : IHandleOrder<OrderPlaced>, IHandleOrder<OrderCooked>, IHandleOrder<OrderPriced>, IHandleOrder<OrderPaid>
    {
        string CorrelationId { get; }
    }
}