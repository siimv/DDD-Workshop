namespace AdvancedCQRS.DocumentMessaging
{
    public interface IHandleOrder<in T>
        where T : IMessage
    {
        void Handle(T order);
    }
}