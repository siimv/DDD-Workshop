namespace AdvancedCQRS.DocumentMessaging
{
    public class Midget : IHandleOrder<OrderPlaced>, IHandleOrder<OrderCooked>, IHandleOrder<OrderPriced>, IHandleOrder<OrderPaid>
    {
        private readonly string _correlationId;
        private readonly MidgetHouse _midgetHouse;
        private readonly IPublisher _publisher;

        public Midget(string correlationId, MidgetHouse midgetHouse, IPublisher publisher)
        {
            _correlationId = correlationId;
            _midgetHouse = midgetHouse;
            _publisher = publisher;
        }

        public string CorrelationId => _correlationId;

        public void Handle(OrderPlaced order)
        {
            var message = new CookFood { Order = order.Order };
            message.ReplyTo(order);

            _publisher.Publish(message);
        }

        public void Handle(OrderCooked order)
        {
            var message = new PriceOrder { Order = order.Order };
            message.ReplyTo(order);

            _publisher.Publish(message);
        }

        public void Handle(OrderPriced order)
        {
            var message = new TakePayment { Order = order.Order };
            message.ReplyTo(order);

            _publisher.Publish(message);
        }

        public void Handle(OrderPaid order)
        {
            _midgetHouse.KillMidget(this);
        }
    }
}