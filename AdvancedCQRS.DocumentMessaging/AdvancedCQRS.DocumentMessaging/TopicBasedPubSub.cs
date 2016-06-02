using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    public class TopicBasedPubSub : IPublisher
    {
        private readonly Dictionary<string, IHandleOrder> _subscriptions = new Dictionary<string, IHandleOrder>();

        public void Publish(string topic, JObject order)
        {
            if (_subscriptions.ContainsKey(topic))
            {
                _subscriptions[topic].Handle(order);
            }
        }

        public void Subscribe(string topic, IHandleOrder handler)
        {
            _subscriptions.Add(topic, handler);
        }
    }

    public interface IPublisher
    {
        void Publish(string topic, JObject order);
    }
}