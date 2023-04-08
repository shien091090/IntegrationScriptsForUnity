using System;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using Sirenix.OdinInspector;
using SNShien.Common.ArchitectureTools;
using SNShien.Common.DataTools;

namespace SNShien.Common.AudioTools
{
    [Serializable]
    public struct AudioEventCollection
    {
        [ValueDropdown("GetArchitectureEventList")]
        public string triggerEventTypeName;

        public EventReference audioEventReference;

        private Dictionary<string, Type> architectureEventTypeNameDict;

        public Type GetTriggerEventTypeName()
        {
            if (architectureEventTypeNameDict == null)
                InitEventTypeNameDict();

            return architectureEventTypeNameDict[triggerEventTypeName];
        }

        public EventReference GetAudioEventReference => audioEventReference;

        public IEnumerable<string> GetArchitectureEventList()
        {
            if (architectureEventTypeNameDict == null)
                InitEventTypeNameDict();

            List<string> typeNames = architectureEventTypeNameDict.Keys.ToList();
            return typeNames;
        }

        private void InitEventTypeNameDict()
        {
            if (ReflectionManager.HaveAssemblyStorageSource(typeof(IArchitectureEvent)) == false)
                ReflectionManager.AddAssemblyStorage(typeof(IArchitectureEvent));

            List<Type> types = ReflectionManager.GetInheritedTypes<IArchitectureEvent>().ToList();
            types.Remove(typeof(IArchitectureEvent));

            architectureEventTypeNameDict = types.ToDictionary(x => x.Name);
        }
    }
}