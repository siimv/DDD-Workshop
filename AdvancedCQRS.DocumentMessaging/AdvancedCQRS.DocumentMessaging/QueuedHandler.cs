using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    public class QueuedHandler : IHandleOrder, IStartable
    {
        private readonly IHandleOrder _handler;
        private readonly ConcurrentQueue<JObject> _messages = new ConcurrentQueue<JObject>();

        public QueuedHandler(IHandleOrder handler)
        {
            _handler = handler;
        }

        public void Start()
        {
            new Thread(Run).Start();
        }

        private void Run()
        {
            while (true)
            {
                JObject message;
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

        public void Handle(JObject order)
        {
            _messages.Enqueue(order);
        }
    }
}