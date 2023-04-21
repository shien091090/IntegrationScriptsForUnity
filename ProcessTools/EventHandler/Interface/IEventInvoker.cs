namespace SNShien.Common.ProcessTools
{
    public interface IEventInvoker
    {
        void SendEvent<T>(T eventInfo) where T : IArchitectureEvent;
    }
}