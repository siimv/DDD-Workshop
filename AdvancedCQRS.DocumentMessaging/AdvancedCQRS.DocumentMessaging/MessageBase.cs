using System;

namespace AdvancedCQRS.DocumentMessaging
{
    public abstract class MessageBase : IMessage
    {
        public Guid Id { get; set; }
    }

    public interface IMessage
    {
    }
}