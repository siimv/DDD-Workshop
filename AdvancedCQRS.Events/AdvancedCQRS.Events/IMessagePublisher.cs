namespace AdvancedCQRS.Events
{
    public interface IMessagePublisher
    {
        void Publish(IMessage message);
    }
}