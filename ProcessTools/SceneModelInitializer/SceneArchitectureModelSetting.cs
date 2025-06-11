#if CUSTOM_USING_ODIN
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

        [SerializeField] [OnValueChanged("OnModelDefineListChanged")]
        private List<ArchitectureModelDefine> modelDefineList;

        public string SceneName => sceneName;
        public List<ArchitectureModelDefine> ModelDefineList => modelDefineList;

        public SceneArchitectureModelSetting(string sceneName)
        {
            this.sceneName = sceneName;
            modelDefineList = new List<ArchitectureModelDefine>();
        }

        public int GetModelOrder(string modelName)
        {
            return ModelDefineList.FindIndex(x => x.GetModelName == modelName);
        }

        public void AddModelDefine(ArchitectureModelDefine newModelDefine)
        {
            modelDefineList ??= new List<ArchitectureModelDefine>();
            modelDefineList.Add(newModelDefine);
            RefreshModelDefineListOrderNum();
        }

        public bool IsModelDefineExist(string typeName)
        {
            if (ModelDefineList == null || ModelDefineList.Count == 0)
                return false;
            else
                return ModelDefineList.Any(x => x.GetModelName == typeName);
        }

        private void RefreshModelDefineListOrderNum()
        {
            if (ModelDefineList == null || ModelDefineList.Count == 0)
                return;

            for (int i = 0; i < ModelDefineList.Count; i++)
            {
                ArchitectureModelDefine modelDefine = ModelDefineList[i];
                modelDefine.SetOrderNum(i);
            }
        }

        private void OnModelDefineListChanged()
        {
            RefreshModelDefineListOrderNum();
        }

        [OnInspectorInit]
        private void OnInspectorInit()
        {
            RefreshModelDefineListOrderNum();
        }
    }
}
#endif