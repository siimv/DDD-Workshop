using System;

namespace AdvancedCQRS.DocumentMessaging
{
    public class DeplayedSend<T> : MessageBase
        where T : IMessage
    {
        public T Message { get; set; }

        public TimeSpan Delay { get; set; }
    }
}