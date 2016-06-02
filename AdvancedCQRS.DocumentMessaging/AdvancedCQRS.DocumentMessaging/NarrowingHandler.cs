namespace AdvancedCQRS.DocumentMessaging
{
    public class NarrowingHandler<TInput, TOutput> : IHandleOrder<TInput>
        where TInput : IMessage
        where TOutput : TInput
    {
        private readonly IHandleOrder<TOutput> _handler;

        public NarrowingHandler(IHandleOrder<TOutput> handler)
        {
            _handler = handler;
        }

        public void Handle(TInput message)
        {
            _handler.Handle((TOutput)message); // will throw if message type is wrong
        }
    }
}