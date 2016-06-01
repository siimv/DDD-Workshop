namespace AdvancedCQRS.Events.Tests
{
    internal static class TestHelper
    {
        public static T Get<T>(this SendMessageIn message)
            where T : class, IMessage
        {
            return message?.MessageToSend as T;
        }
    }
}