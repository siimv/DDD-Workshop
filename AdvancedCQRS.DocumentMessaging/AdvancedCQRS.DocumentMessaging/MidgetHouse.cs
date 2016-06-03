using System.Collections.Generic;

namespace AdvancedCQRS.DocumentMessaging
{
    public class MidgetHouse : IHandleOrder<OrderPlaced>
    {
        private readonly Dictionary<string, Midget> _midgets = new Dictionary<string, Midget>();
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

            midget.Handle(order);
        }

        public void KillMidget(Midget midget)
        {
            if (midget == null || !_midgets.ContainsKey(midget.CorrelationId)) return;
            
            _pubsub.Unsubscribe<OrderCooked>(midget.CorrelationId, midget);
            _pubsub.Unsubscribe<OrderPriced>(midget.CorrelationId, midget);
            _pubsub.Unsubscribe<OrderPaid>(midget.CorrelationId, midget);

            _midgets.Remove(midget.CorrelationId);
        }

        private Midget GetMidget(IMessage message)
        {
            if (!_midgets.ContainsKey(message.CorrelationId))
            {
                _midgets[message.CorrelationId] = new Midget(message.CorrelationId, this, _pubsub);
            }

            return _midgets[message.CorrelationId];
        }
    }
}