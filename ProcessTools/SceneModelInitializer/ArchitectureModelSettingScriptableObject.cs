using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SNShien.Common.ProcessTools
{
    [CreateAssetMenu(fileName = "ArchitectureModelSetting", menuName = "SNShien/Create ArchitectureModelSetting")]
    public class ArchitectureModelSettingScriptableObject : SerializedScriptableObject, IArchitectureModelSetting
    {
        private const string ASSET_FOLDER_PATH = @"Assets";

        [SerializeField] private string[] preLoadAssemblyNames;
        [SerializeField] private SceneProcessScriptableObject sceneProcessSetting;
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

        [Button("Init Scene Model Setting List")]
        public void InitSceneModelSettingList()
        {
#if UNITY_EDITOR

            if (sceneProcessSetting == null)
                CheckAutoSetSceneProcessSettingField();

            if (sceneProcessSetting != null)
                CheckAutoSetSceneModelSettingList();

#endif
        }

        private bool IsSceneModelSettingExist(string sceneName)
        {
            return sceneModelSettingList.Any(x => x.SceneName == sceneName);
        }

        private void CheckAutoSetSceneModelSettingList()
        {
            List<string> sceneNameList = sceneProcessSetting.GetSceneNames;
            sceneModelSettingList ??= new List<SceneArchitectureModelSetting>();

            foreach (string sceneName in sceneNameList)
            {
                if (IsSceneModelSettingExist(sceneName) == false)
                    AddSceneModelSetting(sceneName);
            }
        }

        private void CheckAutoSetSceneProcessSettingField()
        {
            string[] guids = AssetDatabase.FindAssets("t:ScriptableObject", new string[] { ASSET_FOLDER_PATH });
            foreach (string guid in guids)
            {
                string objectPath = AssetDatabase.GUIDToAssetPath(guid);
                ScriptableObject scriptableObject = AssetDatabase.LoadAssetAtPath(objectPath, typeof(ScriptableObject)) as ScriptableObject;
                if (scriptableObject == null || scriptableObject.GetType() != typeof(SceneProcessScriptableObject))
                    continue;

                SceneProcessScriptableObject setting = scriptableObject as SceneProcessScriptableObject;

                if (setting != null)
                {
                    sceneProcessSetting = setting;
                    break;
                }
            }
        }

        private void AddSceneModelSetting(string sceneName)
        {
            if (sceneModelSettingList == null)
                sceneModelSettingList = new List<SceneArchitectureModelSetting>();

            sceneModelSettingList.Add(new SceneArchitectureModelSetting(sceneName));
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