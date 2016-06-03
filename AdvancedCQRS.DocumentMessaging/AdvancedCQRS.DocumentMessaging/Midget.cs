using System;
using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    public class Midget : IMidget
    {
        private readonly MidgetHouse _midgetHouse;
        private readonly IPublisher _publisher;
        private bool _isCooked;

        public Midget(string correlationId, MidgetHouse midgetHouse, IPublisher publisher)
        {
            CorrelationId = correlationId;
            _midgetHouse = midgetHouse;
            _publisher = publisher;
        }

        public string CorrelationId { get; }

        public void Handle(OrderPlaced order)
        {
            StartCooking(order, order.Order);
        }

        private void StartCooking(MessageBase message, JObject order)
        {
            var cookFood = new CookFood { Order = order };
            cookFood.ReplyTo(message);
            _publisher.Publish(cookFood);

            var retryMessage = new RetryCooking { Order = order };
            retryMessage.ReplyTo(message);
            var delayedMessage = new DeplayedSend<RetryCooking>
            {
                Message = retryMessage,
                Delay = TimeSpan.FromSeconds(10)
            };
            delayedMessage.ReplyTo(message);
            _publisher.Publish(delayedMessage);
        }

        public void Handle(OrderCooked order)
        {
            _isCooked = true;

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

        public void Handle(RetryCooking order)
        {
            if (_isCooked) return;

            StartCooking(order, order.Order);
        }
    }
}