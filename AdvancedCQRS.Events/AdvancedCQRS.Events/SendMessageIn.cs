namespace AdvancedCQRS.Events
{
    public class SendMessageIn : IMessage
    {
        public IMessage MessageToSend { get; set; }

        public override string ToString()
        {
            return $"Message: {{{MessageToSend.GetType().Name}: {MessageToSend} }}";
        }
    }
}