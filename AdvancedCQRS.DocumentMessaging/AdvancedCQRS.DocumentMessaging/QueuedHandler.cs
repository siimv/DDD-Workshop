using System.Collections.Concurrent;
using System.Threading;

namespace AdvancedCQRS.DocumentMessaging
{
    public class QueuedHandler
    {
        public static QueuedHandler<T> Create<T>(IHandleOrder<T> handler, string name) where T : IMessage
        {
            return new QueuedHandler<T>(name, handler);
        }
    }

    public class QueuedHandler<T> : IHandleOrder<T>, IStartable, IQueue where T : IMessage
    {
        private readonly string _name;
        private readonly IHandleOrder<T> _handler;
        private readonly ConcurrentQueue<T> _messages = new ConcurrentQueue<T>();

        public QueuedHandler(string name, IHandleOrder<T> handler)
        {
            _name = name;
            _handler = handler;
        }

        public string Name => _name;

        public int NumberOfMessages => _messages.Count;

        public void Start()
        {
            new Thread(Run).Start();
        }

        private void Run()
        {
            while (true)
            {
                T message;
                if (_messages.TryDequeue(out message))
                {
                    _handler.Handle(message);
                }
                else
                {
                    Thread.Sleep(1);
                }
            }
        }

        public void Handle(T order)
        {
            _messages.Enqueue(order);
        }
    }
}