using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    public class Program
    {
        public static void Main()
        {
            var cashier = new QueuedHandler(new Cashier(new PrintingOrderHandler()));
            var manager = new QueuedHandler(new Manager(cashier));
            var cook1 = new QueuedHandler(new Cook(manager));
            var cook2 = new QueuedHandler(new Cook(manager));
            var cook3 = new QueuedHandler(new Cook(manager));
            var cooks = new RoundRobinDispatcher(cook1, cook2, cook3);
            var waiter = new Waiter(cooks);

            for (int i = 1; i < 11; i++)
            {
                waiter.TakeOrder(i, CreateOrder());
            }

            cashier.Start();
            manager.Start();
            cook1.Start();
            cook2.Start();
            cook3.Start();
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