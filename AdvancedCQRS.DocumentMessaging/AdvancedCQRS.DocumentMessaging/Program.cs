using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    public class Program
    {
        public static void Main()
        {
            var cashier = new Cashier(new PrintingOrderHandler());
            var manager = new Manager(cashier);
            var cook = new Cook(manager);
            var waiter = new Waiter(cook);

            for (int i = 1; i < 11; i++)
            {
                waiter.TakeOrder(i, CreateOrder());
            }
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