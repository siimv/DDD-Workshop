using System.Collections.Generic;
using System.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    public class TopicBasedPubSub : IPublisher
    {
        private object _lock = new object();
        private readonly Dictionary<string, List<object>> _subscriptions = new Dictionary<string, List<object>>();
        
        public void SubscribeByMessage<T>(IHandleOrder<T> handler) where T : IMessage
        {
            var topic = GetTopic<T>();
            Subscribe(topic, handler);
        }

        public void SubscribeByCorrelationId<T>(string correlationId, IHandleOrder<T> handler) where T : IMessage
        {
            Subscribe(correlationId, handler);
        }

        private void Subscribe<T>(string topic, IHandleOrder<T> handler) where T : IMessage
        {
            lock (_lock)
            {
                var subscriptions = CopySubscribers(topic);
                if (subscriptions.Contains(handler)) return;

                subscriptions.Add(handler);

                ReplaceSubscribers(topic, subscriptions);
            }
        }

        public void Unsubscribe<T>(IHandleOrder<T> handler) where T : IMessage
        {
            var topic = GetTopic<T>();
            Unsubscribe(topic, handler);
        }

        public void Unsubscribe<T>(string topic, IHandleOrder<T> handler) where T : IMessage
        {
            lock (_lock)
            {
                var subscriptions = CopySubscribers(topic);
                subscriptions.Remove(handler);

                ReplaceSubscribers(topic, subscriptions);
            }
        }

        private void ReplaceSubscribers(string topic, List<object> subscribers)
        {
            _subscriptions[topic] = subscribers;
        }

        private List<object> CopySubscribers(string topic)
        {
            var existing = _subscriptions.ContainsKey(topic) ? _subscriptions[topic] : Enumerable.Empty<object>();
            var subscriptions = new List<object>(existing);
            return subscriptions;
        }

        public void Publish<T>(T message) where T : IMessage
        {
            var topic = GetTopic<T>();

            Publish(topic, message);
            Publish(message.CorrelationId, message);
        }

        private void Publish<T>(string topic, T message) where T : IMessage
        {
            if (!_subscriptions.ContainsKey(topic)) return;

            foreach (var handler in _subscriptions[topic].OfType<IHandleOrder<T>>())
            {
                var localHandle = handler;
                localHandle.Handle(message);
            }
        }

        private static string GetTopic<T>() where T : IMessage
        {
            var topic = typeof(T).Name;
            return topic;
        }
    }

    public interface IPublisher
    {
        void Publish<T>(T message)
            where T : IMessage;
    }
}