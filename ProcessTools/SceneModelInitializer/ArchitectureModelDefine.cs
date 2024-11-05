using System.Reflection;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SNShien.Common.ProcessTools
{
    [System.Serializable]
    public class ArchitectureModelDefine
    {
        [SerializeField] private string modelName;
        [SerializeField] private int orderNum;

        public string GetModelName => modelName;
        public int GetOrderNum => orderNum;

        public ArchitectureModelDefine(MemberInfo type, int orderNum)
        {
            this.modelName = type.Name;
            this.orderNum = orderNum;
        }

        public void SetOrderNum(int orderNum)
        {
            this.orderNum = orderNum;
        }
    }
}