using System;
using Newtonsoft.Json.Linq;

namespace AdvancedCQRS.DocumentMessaging
{
    public class PrintingOrderHandler : IHandleOrder
    {
        public void Handle(JObject order)
        {
            Console.WriteLine(order);
        }
    }
}