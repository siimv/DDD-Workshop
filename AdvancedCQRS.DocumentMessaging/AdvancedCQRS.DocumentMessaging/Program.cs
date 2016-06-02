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
            //var handler = new PrintingOrderHandler();
            var handler = new NullHandler();

            var cashier = new QueuedHandler("Cashier #1", new Cashier(handler));
            var manager = new QueuedHandler("Manager #1", new Manager(cashier));
            var cook1 = new QueuedHandler("Cook #1", new Cook("Tom", manager, Random.Next(0, 1000)));
            var cook2 = new QueuedHandler("Cook #2", new Cook("Jones", manager, Random.Next(0, 1000)));
            var cook3 = new QueuedHandler("Cook #3", new Cook("Huck", manager, Random.Next(0, 1000)));
            var cooks = new RoundRobinDispatcher(cook1, cook2, cook3);
            var waiter = new Waiter(cooks);

            var startables = new []{ cashier, manager, cook1, cook2, cook3 };

            foreach (var startable in startables)
            {
                startable.Start();
            }

            StartMonitoring(startables);

            for (int i = 1; i < 300; i++)
            {
                waiter.TakeOrder(i, CreateOrder());
            }
        }

        private static void StartMonitoring(IList<QueuedHandler> handlers)
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