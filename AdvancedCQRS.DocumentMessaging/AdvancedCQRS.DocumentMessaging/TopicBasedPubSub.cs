using System.Collections.Generic;

namespace AdvancedCQRS.DocumentMessaging
{
    public class TopicBasedPubSub : IPublisher
    {
        private readonly Dictionary<string, IHandleOrder<IMessage>> _subscriptions = new Dictionary<string, IHandleOrder<IMessage>>();
        
        public void Subscribe<T>(IHandleOrder<T> handler) where T : IMessage
        {
            var topic = GetTopic<T>();
            _subscriptions.Add(topic, new NarrowingHandler<IMessage, T>(handler));
        }

        public void Subscribe<T>(string topic, IHandleOrder<T> handler) where T : IMessage
        {
            _subscriptions.Add(topic, new NarrowingHandler<IMessage, T>(handler));
        }

        public void Publish<T>(T message) where T : IMessage
        {
            var topic = GetTopic<T>();
            if (_subscriptions.ContainsKey(topic))
            {
                _subscriptions[topic].Handle(message);
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