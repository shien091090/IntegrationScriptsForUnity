using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SNShien.Common.ProcessTools
{
    [System.Serializable]
    public class SceneArchitectureModelSetting : ISceneArchitectureModelSetting
    {
        [SerializeField] private string sceneName;
        [SerializeField] private List<ArchitectureModelDefine> modelDefineList;

        public string SceneName => sceneName;

        public int GetModelOrder(string modelName)
        {
            return modelDefineList.FirstOrDefault(x => x.GetModelName == modelName)?.GetOrderNum ?? 0;
        }

        private bool IsModelDefineExist(string typeName)
        {
            if (modelDefineList == null || modelDefineList.Count == 0)
                return false;
            else
                return modelDefineList.Any(x => x.GetModelName == typeName);
        }
    }
}