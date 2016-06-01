using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedCQRS.Events
{
    public class FakeMessagePublisher : IMessagePublisher
    {
        private readonly List<IMessage> _messages = new List<IMessage>();

        public void Publish(IMessage message)
        {
            _messages.Add(message);
            Console.WriteLine($"{message.GetType().Name}: {message}");
        }

        public T FindMessage<T>()
            where T : IMessage
        {
            return _messages.OfType<T>().SingleOrDefault();
        }

        public T FindSendMeInMessage<T>()
            where T : IMessage
        {
            return _messages.OfType<SendMessageIn>().Select(x => x.MessageToSend).OfType<T>().SingleOrDefault();
        }

        public void Clear()
        {
            _messages.Clear();
        }
    }
}