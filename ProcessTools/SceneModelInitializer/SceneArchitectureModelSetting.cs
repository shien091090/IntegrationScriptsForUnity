using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SNShien.Common.ProcessTools
{
    [System.Serializable]
    public class SceneArchitectureModelSetting : ISceneArchitectureModelSetting
    {
        [SerializeField] [ReadOnly] private string sceneName;
        [SerializeField] private List<ArchitectureModelDefine> modelDefineList;

        public string SceneName => sceneName;

        public SceneArchitectureModelSetting(string sceneName)
        {
            this.sceneName = sceneName;
            modelDefineList = new List<ArchitectureModelDefine>();
        }

        public int GetModelOrder(string modelName)
        {
            return modelDefineList.FindIndex(x => x.GetModelName == modelName);
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