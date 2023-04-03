using System;
using System.Collections.Generic;

namespace SNShien.Common.ArchitectureTools
{
    public class ArchitectureEventHandler : IEventInvoker, IEventRegister
    {
        private readonly Dictionary<Type, object> eventDict = new Dictionary<Type, object>();

        public void SendEvent<T>(params object[] inputParams) where T : IArchitectureEvent
        {
            if (!eventDict.ContainsKey(typeof(T)))
                return;
            
            T paramInstance = (T)Activator.CreateInstance(typeof(T), inputParams);
            Action<T> eventAction = (Action<T>)eventDict[typeof(T)];
            eventAction?.Invoke(paramInstance);
        }

        public void Register<T>(Action<T> eventCallback) where T : IArchitectureEvent
        {
            Action<T> eventData = null;
            if (eventDict.ContainsKey(typeof(T))) eventData = (Action<T>)eventDict[typeof(T)];
            eventData += eventCallback;
            eventDict[typeof(T)] = eventData;
        }

        public void Unregister<T>(Action<T> eventCallback) where T : IArchitectureEvent
        {
            if (eventDict.ContainsKey(typeof(T)) == false) return;
            Action<T> eventData = (Action<T>)eventDict[typeof(T)];
            eventData -= eventCallback;
            if (eventData == null) eventDict.Remove(typeof(T));
            else eventDict[typeof(T)] = eventData;
        }
    }
}