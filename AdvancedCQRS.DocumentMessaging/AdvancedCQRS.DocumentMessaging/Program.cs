using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    public class Program
    {
        private static readonly Random Random = new Random();

        public static void Main()
        {
            var pubsub = new TopicBasedPubSub();

            var waiter = new Waiter(pubsub);

            var cook1 = QueuedHandler.Create(new Cook("Tom", pubsub, Random.Next(0, 1000)), "Cook #1");
            var cook2 = QueuedHandler.Create(new Cook("Jones", pubsub, Random.Next(0, 1000)), "Cook #2");
            var cook3 = QueuedHandler.Create(new Cook("Huck", pubsub, Random.Next(0, 1000)), "Cook #3");
            var kitchen = QueuedHandler.Create(MoreFareDispatcher.Create(cook1, cook2, cook3), "Kitchen");
            pubsub.Subscribe(kitchen);

            var manager = QueuedHandler.Create(new Manager(pubsub), "Manager #1");
            pubsub.Subscribe(manager);

            var cashier = QueuedHandler.Create(new Cashier(pubsub), "Cashier #1");
            pubsub.Subscribe(cashier);

            //pubsub.Subscribe(new PrintingOrderHandler());

            var startables = new IStartable[]{ kitchen, cook1, cook2, cook3, cashier, manager };
            var queues = new IQueue[]{ kitchen, cook1, cook2, cook3, cashier, manager };

            foreach (var startable in startables)
            {
                startable.Start();
            }

            StartMonitoring(queues);
            TakeOrders(waiter);
        }

        private static void TakeOrders(Waiter waiter)
        {
            for (int i = 1; i < 300; i++)
            {
                waiter.TakeOrder(i, CreateOrder());
            }
        }

        private static void StartMonitoring(IList<IQueue> handlers)
        {
            new Thread(() =>
            {
                while (true)
                {
                    Console.WriteLine("-----------------");
                    foreach (var handler in handlers)
                    {
                        Console.WriteLine($"{handler.Name}: {handler.NumberOfMessages}");
                    }
                    Console.WriteLine("-----------------");
                    Thread.Sleep(1000);
                }
            }).Start();
        }

        private static IEnumerable<LineItem> CreateOrder()
        {
            return new[]
            {
                new LineItem(new JObject())
                { Item = "razor blade ice cream", Quantity = 2, Price = 399 },
                new LineItem(new JObject())
                {
                    Item = "meat burger",
                    Quantity = 1,
                    Price = 550
                },
                new LineItem(new JObject())
                {
                    Item = "pizza",
                    Quantity = 1,
                    Price = 550
                },
            };
        }
    }
}