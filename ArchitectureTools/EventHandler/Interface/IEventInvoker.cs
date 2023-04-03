namespace SNShien.Common.ArchitectureTools
{
    public interface IEventInvoker
    {
        void SendEvent<T>(params object[] inputParams) where T : IArchitectureEvent;
    }
}