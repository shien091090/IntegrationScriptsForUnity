using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SNShien.Common.ProcessTools
{
    [CreateAssetMenu(fileName = "ArchitectureModelSetting", menuName = "SNShien/Create ArchitectureModelSetting")]
    public class ArchitectureModelSettingScriptableObject : SerializedScriptableObject, IArchitectureModelSetting
    {
        [SerializeField] private string[] preLoadAssemblyNames;
        [SerializeField] private List<SceneArchitectureModelSetting> sceneModelSettingList;


        public ISceneArchitectureModelSetting GetModelSetting(string sceneName)
        {
            if (sceneModelSettingList == null || sceneModelSettingList.Count == 0)
                return null;

            List<SceneArchitectureModelSetting> matchSettings = sceneModelSettingList
                .Where(x => x.SceneName == sceneName)
                .ToList();

            return matchSettings.Count == 0 ?
                null :
                matchSettings[0];
        }

        // [Button("Parse Model Define List")]
        // private void ParseModelDefineList()
        // {
        //     if (preLoadAssemblyNames != null && preLoadAssemblyNames.Length > 0)
        //     {
        //         foreach (string assemblyName in preLoadAssemblyNames)
        //             ReflectionManager.AddAssemblyStorage(assemblyName);
        //     }
        //
        //     if (ReflectionManager.HaveAssemblyStorageSource(typeof(IArchitectureModel)) == false)
        //         ReflectionManager.AddAssemblyStorage(typeof(IArchitectureModel));
        //
        //     List<Type> modelTypes = ReflectionManager.GetInheritedTypes<IArchitectureModel>().ToList();
        //     modelTypes.Remove(typeof(IArchitectureModel));
        //     modelTypes = modelTypes.Where(x => x.IsInterface == false).ToList();
        //
        //     for (int i = 0; i < modelTypes.Count; i++)
        //     {
        //         Type type = modelTypes[i];
        //         if (IsModelDefineExist(type.Name) == false)
        //             AddDefineList(type, i + 1);
        //     }
        // }

        // private void AddDefineList(Type type, int orderNum)
        // {
        //     if (modelDefineList == null)
        //         modelDefineList = new List<ArchitectureModelDefine>();
        //
        //     modelDefineList.Add(new ArchitectureModelDefine(type, orderNum));
        // }

        // private void ReParseOrderNum()
        // {
        //     for (int i = 0; i < modelDefineList.Count; i++)
        //     {
        //         ArchitectureModelDefine modelDefine = modelDefineList[i];
        //         modelDefine.SetOrderNum(i + 1);
        //     }
        // }
    }
}