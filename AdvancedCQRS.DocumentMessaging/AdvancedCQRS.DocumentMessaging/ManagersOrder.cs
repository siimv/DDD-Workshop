using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    public class Manager : IHandleOrder
    {
        private readonly IPublisher _publisher;

        public Manager(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public void Handle(JObject baseOrder)
        {
            var order = new ManagersOrder(baseOrder);

            var totalWithoutTax = order.Items.Sum(item => item.Quantity * item.Price);
            var tax = (int)(totalWithoutTax * 0.2);

            order.Tax = tax;
            order.Total = totalWithoutTax + tax;

            _publisher.Publish("TotalCalculated", order.InnerItem);
        }
    }

    public class ManagersOrder
    {
        private readonly JObject _order;

        public ManagersOrder(JObject order)
        {
            _order = (JObject)order.DeepClone();
        }

        public JObject InnerItem
        {
            get { return _order; }
        }

        public int Tax
        {
            get { return (int)_order["Tax"]; }
            set { _order["Tax"] = value; }
        }

        public int Total
        {
            get { return (int)_order["Total"]; }
            set { _order["Total"] = value; }
        }

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
    }
}