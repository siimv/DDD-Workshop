using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    public class Cook : IHandleOrder
    {
        private readonly IHandleOrder _orderHandler;

        public Cook(IHandleOrder orderHandler)
        {
            _orderHandler = orderHandler;
        }

        private readonly Dictionary<string, string> _ingredientsMap = new Dictionary<string, string>
        {
            {"razor blade ice cream", "razor blades, ice cream" },
            {"random1", "meat, tomatos" },
            {"random2", "onions, olives" },
            {"random3", "secret" },
        };

        public void Handle(JObject baseOrder)
        {
            var order = new CooksOrder(baseOrder);

            var timeToCook = TimeToCook(string.Join(" ", order.Items.Select(x => x.Item)));
            Thread.Sleep(timeToCook);

            order.Ingredients = string.Join(", ", order.Items.Select(FindIngredients));
            order.CookedAt = DateTime.Now;

            _orderHandler.Handle(order.InnerItem);
        }

        private string FindIngredients(LineItem item)
        {
            if (_ingredientsMap.ContainsKey(item.Item))
            {
                return _ingredientsMap[item.Item];
            }
            
            return _ingredientsMap[$"random{new Random().Next(1, 3)}"];
        }

        private int TimeToCook(string order)
        {
            if (order.Contains("burger")) return 3000;
            if (order.Contains("pancake")) return 1000;
            return 2000;
        }
    }

    public class CooksOrder
    {
        private readonly JObject _order;

        public CooksOrder(JObject order)
        {
            _order = (JObject)order.DeepClone();
        }

        public JObject InnerItem => _order;

        public IEnumerable<LineItem> Items
        {
            get
            {
                foreach (JObject item in _order["LineItems"] ?? new JArray())
                {
                    yield return new LineItem(item);
                }
            }
        }

        public DateTime CookedAt
        {
            set { _order["CookedAt"] = value; }
        }

        public string Ingredients
        {
            get { return (string)_order["Ingredients"]; }
            set { _order["Ingredients"] = value; }
        }
    }
}