using System;

namespace SNShien.Common.ArchitectureTools
{
    public interface IEventRegister
    {
        void Register<T>(Action<T> eventAction) where T : IArchitectureEvent;
        void Unregister<T>(Action<T> eventAction) where T : IArchitectureEvent;
    }
}