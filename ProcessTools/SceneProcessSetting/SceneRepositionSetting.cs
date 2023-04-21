using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SNShien.Common.ProcessTools
{
    public class SceneRepositionSetting
    {
        [SerializeField] private string repositionActionKey;

        [SerializeField] [ValueDropdown("GetSceneNames")]
        private string loadSceneName;

        [SerializeField] [ValueDropdown("GetSceneNames")]
        private string unloadSceneName;

        private string[] GetSceneNames;
        public string GetRepositionActionKey => repositionActionKey;
        public string GetLoadSceneName => loadSceneName == SceneProcessScriptableObject.EMPTY_SCENE_NAME ? string.Empty : loadSceneName;
        public string GetUnloadSceneName => unloadSceneName == SceneProcessScriptableObject.EMPTY_SCENE_NAME ? string.Empty : unloadSceneName;


        public SceneRepositionSetting()
        {
        }

        public void SetSceneNames(List<string> newSceneNames)
        {
            GetSceneNames = newSceneNames.ToArray();
        }
    }
}