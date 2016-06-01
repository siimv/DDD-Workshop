using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace AdvancedCQRS.DocumentMessaging.Tests
{
    public class WaitersOrderTests
    {
        [Test]
        public void It_should_add_first_line_item()
        {
            var order = new WaitersOrder(new JObject());

            order.AddItem(new LineItem(new JObject())
            {
                Item = "Burger"
            });

            var items = order.InnerItem["LineItems"] as JArray;
            Assert.That(items.First != null);
            Assert.That(items.First is JObject);
        }
    }
}