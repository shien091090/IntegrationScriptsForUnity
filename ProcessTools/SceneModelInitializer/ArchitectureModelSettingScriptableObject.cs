using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using SNShien.Common.DataTools;
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

        [Button("Auto Create Settings")]
        public void AutoCreateSettings()
        {
#if UNITY_EDITOR

            if (sceneProcessSetting == null)
                CheckAutoSetSceneProcessSettingField();

            if (sceneProcessSetting != null)
                CheckAutoSetSceneModelSettingList();

            CheckAutoAddModelDefine();
#endif
        }

        private bool IsSceneModelSettingExist(string sceneName)
        {
            return sceneModelSettingList.Any(x => x.SceneName == sceneName);
        }

        private bool IsModelDefineExist(string typeName)
        {
            if (sceneModelSettingList == null || sceneModelSettingList.Count == 0)
                return false;
            
            return sceneModelSettingList.Any(sceneModelSetting => sceneModelSetting.IsModelDefineExist(typeName));
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

        private void CheckAutoAddModelDefine()
        {
            if (preLoadAssemblyNames != null && preLoadAssemblyNames.Length > 0)
            {
                foreach (string assemblyName in preLoadAssemblyNames)
                    ReflectionManager.AddAssemblyStorage(assemblyName);
            }

            if (ReflectionManager.HaveAssemblyStorageSource(typeof(IArchitectureModel)) == false)
                ReflectionManager.AddAssemblyStorage(typeof(IArchitectureModel));

            List<Type> modelTypes = ReflectionManager.GetInheritedTypes<IArchitectureModel>().ToList();
            modelTypes.Remove(typeof(IArchitectureModel));
            modelTypes = modelTypes.Where(x => x.IsInterface == false).ToList();

            foreach (Type type in modelTypes)
            {
                if (IsModelDefineExist(type.Name) == false)
                    AddDefineList(type);
            }
        }

        private void AddSceneModelSetting(string sceneName)
        {
            if (sceneModelSettingList == null)
                sceneModelSettingList = new List<SceneArchitectureModelSetting>();

            sceneModelSettingList.Add(new SceneArchitectureModelSetting(sceneName));
        }

        private void AddDefineList(Type type)
        {
            if (sceneModelSettingList == null || sceneModelSettingList.Count == 0)
                return;

            sceneModelSettingList[0].AddModelDefine(new ArchitectureModelDefine(type.Name));
        }
    }
}