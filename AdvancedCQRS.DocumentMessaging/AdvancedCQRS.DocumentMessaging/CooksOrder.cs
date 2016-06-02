using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    public class Cook : IHandleOrder<OrderPlaced>
    {
        private readonly string _name;
        private readonly IPublisher _publisher;
        private readonly int _cookingTime;

        public Cook(string name, IPublisher publisher, int cookingTime)
        {
            _name = name;
            _publisher = publisher;
            _cookingTime = cookingTime;
        }

        private readonly Dictionary<string, string> _ingredientsMap = new Dictionary<string, string>
        {
            {"razor blade ice cream", "razor blades, ice cream" },
            {"random1", "meat, tomatos" },
            {"random2", "onions, olives" },
            {"random3", "secret" },
        };

        public void Handle(OrderPlaced baseOrder)
        {
            var order = new CooksOrder(baseOrder.Order);
            
            Thread.Sleep(_cookingTime);

            order.Ingredients = string.Join(", ", order.Items.Select(FindIngredients));
            order.CookedAt = DateTime.Now;
            order.CookedBy = _name;

            _publisher.Publish(new OrderCooked { Order = order.InnerItem });
        }

        private string FindIngredients(LineItem item)
        {
            if (_ingredientsMap.ContainsKey(item.Item))
            {
                return _ingredientsMap[item.Item];
            }
            
            return _ingredientsMap[$"random{new Random().Next(1, 3)}"];
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

        public string CookedBy
        {
            set { _order["CookedBy"] = value; }
        }

        public string Ingredients
        {
            get { return (string)_order["Ingredients"]; }
            set { _order["Ingredients"] = value; }
        }
    }
}