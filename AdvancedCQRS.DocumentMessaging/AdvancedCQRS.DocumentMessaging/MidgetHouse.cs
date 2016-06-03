using System.Collections.Generic;

namespace AdvancedCQRS.DocumentMessaging
{
    public class MidgetHouse : IHandleOrder<OrderPlaced>
    {
        private readonly Dictionary<string, IMidget> _midgets = new Dictionary<string, IMidget>();
        private readonly TopicBasedPubSub _pubsub;

        public MidgetHouse(TopicBasedPubSub pubsub)
        {
            _pubsub = pubsub;
        }

        public void Handle(OrderPlaced order)
        {
            var midget = GetMidget(order);
            _pubsub.SubscribeByCorrelationId<OrderCooked>(order.CorrelationId, midget);
            _pubsub.SubscribeByCorrelationId<OrderPriced>(order.CorrelationId, midget);
            _pubsub.SubscribeByCorrelationId<OrderPaid>(order.CorrelationId, midget);
            _pubsub.SubscribeByCorrelationId<RetryCooking>(order.CorrelationId, midget);

            midget.Handle(order);
        }

        public void KillMidget(IMidget midget)
        {
            if (midget == null || !_midgets.ContainsKey(midget.CorrelationId)) return;
            
            _pubsub.Unsubscribe<OrderCooked>(midget.CorrelationId, midget);
            _pubsub.Unsubscribe<OrderPriced>(midget.CorrelationId, midget);
            _pubsub.Unsubscribe<OrderPaid>(midget.CorrelationId, midget);
            _pubsub.Unsubscribe<RetryCooking>(midget.CorrelationId, midget);

            _midgets.Remove(midget.CorrelationId);
        }

        private IMidget GetMidget(OrderPlaced message)
        {
            if (!_midgets.ContainsKey(message.CorrelationId))
            {
                _midgets[message.CorrelationId] = CreateMidget(message);
            }

            return _midgets[message.CorrelationId];
        }

        private IMidget CreateMidget(OrderPlaced message)
        {
            if ((bool)message.Order["IsDodgy"])
            {
                return new ZimbabweMidget(message.CorrelationId, this, _pubsub);
            }
            return new Midget(message.CorrelationId, this, _pubsub);
        }
    }
}