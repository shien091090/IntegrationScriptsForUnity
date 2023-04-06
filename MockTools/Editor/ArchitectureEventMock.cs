using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using SNShien.Common.ArchitectureTools;
using Assert = NUnit.Framework.Assert;

namespace SNShien.Common.MockTools
{
    public class ArchitectureEventMock
    {
        private Mock<IEventInvoker> _eventInvokerMock;
        private Dictionary<Type, List<IArchitectureEvent>> _eventTriggerRecordDict;
        public IEventInvoker GetEventInvoker => _eventInvokerMock.Object;

        public ArchitectureEventMock()
        {
            _eventTriggerRecordDict = new Dictionary<Type, List<IArchitectureEvent>>();
            SetupEventInvokerMock();
        }

        private void SetupEventInvokerMock()
        {
            if (_eventInvokerMock != null)
                return;

            _eventInvokerMock = new Mock<IEventInvoker>();
            _eventInvokerMock
                .Setup(x => x.SendEvent(It.IsAny<IArchitectureEvent>()))
                .Callback((IArchitectureEvent eventInfo) =>
                {
                    Type eventType = eventInfo.GetType();
                    if (_eventTriggerRecordDict.ContainsKey(eventType))
                        _eventTriggerRecordDict[eventType].Add(eventInfo);
                    else
                        _eventTriggerRecordDict[eventType] = new List<IArchitectureEvent>() { eventInfo };
                });
        }

        public void VerifyEventTriggerTimes<T>(int expectedTriggerTimes) where T : IArchitectureEvent
        {
            int times = _eventTriggerRecordDict.ContainsKey(typeof(T)) ?
                _eventTriggerRecordDict[typeof(T)].Count :
                0;

            Assert.AreEqual(expectedTriggerTimes, times);
        }

        public T GetLastTriggerEventInfo<T>() where T : IArchitectureEvent
        {
            if (_eventTriggerRecordDict.ContainsKey(typeof(T)) == false)
                return default;

            List<IArchitectureEvent> architectureEvents = _eventTriggerRecordDict[typeof(T)];
            IArchitectureEvent lastEventInfo = architectureEvents[architectureEvents.Count - 1];
            return (T)lastEventInfo;
        }

        public List<T> GetTriggerEventInfoList<T>() where T : IArchitectureEvent
        {
            if (_eventTriggerRecordDict.ContainsKey(typeof(T)) == false)
                return default;
            else
            {
                List<IArchitectureEvent> triggerEventInfoList = _eventTriggerRecordDict[typeof(T)];
                return triggerEventInfoList.Cast<T>().ToList();
            }
        }

        public void ClearEventRecord<T>()
        {
            if (_eventTriggerRecordDict.ContainsKey(typeof(T)))
                _eventTriggerRecordDict.Remove(typeof(T));
        }
    }
}