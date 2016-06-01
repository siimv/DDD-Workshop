using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    public class Cashier : IHandleOrder
    {
        private readonly IHandleOrder _orderHandler;

        public Cashier(IHandleOrder orderHandler)
        {
            _orderHandler = orderHandler;
        }

        public void Handle(JObject baseOrder)
        {
            var order = new CashiersOrder(baseOrder);
            order.IsPaid = true;

            _orderHandler.Handle(order.InnerItem);
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