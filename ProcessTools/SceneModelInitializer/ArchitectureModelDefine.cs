using System.Reflection;
using Sirenix.OdinInspector;

namespace SNShien.Common.ProcessTools
{
    [System.Serializable]
    public class ArchitectureModelDefine
    {
        [Title("$ModelName")] [ReadOnly] public int orderNum;

        private string modelName;

        public string ModelName => modelName;

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