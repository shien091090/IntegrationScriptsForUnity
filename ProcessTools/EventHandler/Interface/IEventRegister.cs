using System;

namespace SNShien.Common.ProcessTools
{
    public interface IEventRegister
    {
        void Register<T>(Action<T> eventAction) where T : IArchitectureEvent;
        void Unregister<T>(Action<T> eventAction) where T : IArchitectureEvent;
    }
}