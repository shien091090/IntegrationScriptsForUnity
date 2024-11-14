using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SNShien.Common.ProcessTools
{
    [System.Serializable]
    public class ArchitectureModelDefine
    {
        [SerializeField] [SuffixLabel("@OrderNum")] private string modelName;

        public string GetModelName => modelName;
        private int OrderNum { get; set; }

        public ArchitectureModelDefine(string modelName)
        {
            this.modelName = modelName;
        }

        public void SetOrderNum(int orderNum)
        {
            OrderNum = orderNum;
        }
    }
}