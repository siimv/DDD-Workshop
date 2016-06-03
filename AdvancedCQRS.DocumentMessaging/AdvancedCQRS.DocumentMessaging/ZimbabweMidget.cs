namespace AdvancedCQRS.DocumentMessaging
{
    public class ZimbabweMidget : IMidget
    {
        private readonly MidgetHouse _midgetHouse;
        private readonly IPublisher _publisher;

        public ZimbabweMidget(string correlationId, MidgetHouse midgetHouse, IPublisher publisher)
        {
            CorrelationId = correlationId;
            _midgetHouse = midgetHouse;
            _publisher = publisher;
        }

        public string CorrelationId { get; }

        public void Handle(OrderPlaced order)
        {
            var message = new PriceOrder { Order = order.Order };
            message.ReplyTo(order);

            _publisher.Publish(message);
        }

        public void Handle(OrderCooked order)
        {
            _midgetHouse.KillMidget(this);
        }

        public void Handle(OrderPriced order)
        {
            var message = new TakePayment { Order = order.Order };
            message.ReplyTo(order);

            _publisher.Publish(message);
        }

        public void Handle(OrderPaid order)
        {
            var message = new CookFood { Order = order.Order };
            message.ReplyTo(order);

            _publisher.Publish(message);
        }

        public void Handle(RetryCooking order)
        {
        }
    }
}