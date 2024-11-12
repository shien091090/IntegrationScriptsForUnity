using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SNShien.Common.ProcessTools
{
    [System.Serializable]
    public class ArchitectureModelDefine
    {
        [SerializeField] [SuffixLabel("@GetTestNum()")] private string modelName;

        public string GetModelName => modelName;
        
        public int GetTestNum()
        {
            return 0;
        }

        public ArchitectureModelDefine(MemberInfo type)
        {
            modelName = type.Name;
        }
    }
}