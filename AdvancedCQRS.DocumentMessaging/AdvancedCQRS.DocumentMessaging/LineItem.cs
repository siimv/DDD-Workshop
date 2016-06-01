using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    public class LineItem
    {
        private readonly JObject _item;

        public LineItem(JObject item)
        {
            _item = (JObject)item.DeepClone();
        }

        public JObject InnerItem
        {
            get { return _item; }
        }

        public int Quantity
        {
            get { return (int)_item["Quantity"]; }
            set { _item["Quantity"] = value; }
        }

        public string Item
        {
            get { return (string)_item["Item"]; }
            set { _item["Item"] = value; }
        }

        public int Price
        {
            get { return (int)_item["Price"]; }
            set { _item["Price"] = value; }
        }
    }
}