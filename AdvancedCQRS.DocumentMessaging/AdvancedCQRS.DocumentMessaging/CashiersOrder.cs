using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    public class Cashier : IHandleOrder<OrderPriced>
    {
        private readonly IPublisher _publisher;

        public Cashier(IPublisher publisher)
        {
            _publisher = publisher;
        }

        public void Handle(OrderPriced baseOrder)
        {
            var order = new CashiersOrder(baseOrder.Order);
            order.IsPaid = true;

            _publisher.Publish(new OrderPaid { Order = order.InnerItem });
        }
    }

    public class CashiersOrder
    {
        private readonly JObject _order;

        public CashiersOrder(JObject order)
        {
            _order = (JObject)order.DeepClone();
        }

        public JObject InnerItem
        {
            get { return _order; }
        }

        public bool IsPaid
        {
            get { return (bool)_order["IsPaid"]; }
            set { _order["IsPaid"] = value; }
        }
    }


}