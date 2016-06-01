namespace AdvancedCQRS.Events
{
    public abstract class PricedMessage : IMessage
    {
        public int Price { get; set; }

        public override string ToString()
        {
            return $"Price: {Price}";
        }
    }
}