namespace SNShien.Common.ArchitectureTools
{
    public interface IEventInvoker
    {
        void SendEvent<T>(T eventInfo) where T : IArchitectureEvent;
    }
}