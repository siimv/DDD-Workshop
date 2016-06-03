using System;
using System.Collections.Generic;
using System.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    //TODO: Shouldn't be generically typed
    public class AlarmClock<T> : IHandleOrder<DeplayedSend<T>>, IStartable where T : IMessage
    {
        private readonly List<KeyValuePair<DeplayedSend<T>, DateTime>> _list = new List<KeyValuePair<DeplayedSend<T>, DateTime>>();
        private readonly IPublisher _publisher;

        public AlarmClock(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public void Handle(DeplayedSend<T> order)
        {
            _list.Add(new KeyValuePair<DeplayedSend<T>, DateTime>(order, DateTime.Now.Add(order.Delay)));
        }

        private void Run()
        {
            var messagesToSend = _list.Where(x => x.Value <= DateTime.Now).ToList();
            foreach (var message in messagesToSend)
            {
                _publisher.Publish(message.Key.Message);
                _list.Remove(message);
            }
        }

        public void Start()
        {
            var timer = new System.Timers.Timer(1000);
            timer.Elapsed += (s, e) => Run();
            timer.Start();
        }
    }
}