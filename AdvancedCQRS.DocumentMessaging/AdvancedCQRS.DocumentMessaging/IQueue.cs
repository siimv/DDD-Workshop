namespace AdvancedCQRS.DocumentMessaging
{
    public interface IQueue
    {
        string Name { get; }
        int NumberOfMessages { get; }
    }
}