namespace AdvancedCQRS.DocumentMessaging
{
    public interface IMidget : IHandleOrder<OrderPlaced>, IHandleOrder<OrderCooked>, IHandleOrder<OrderPriced>, IHandleOrder<OrderPaid>, IHandleOrder<RetryCooking>
    {
        string CorrelationId { get; }
    }
}