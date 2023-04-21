using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace SNShien.Common.ProcessTools
{
    public class SceneProcessManager : SerializedMonoBehaviour
    {
        [Inject] private ISceneProcessSetting sceneProcessSetting;
        [Inject] private IEventRegister eventRegister;

        [InlineButton("EditorSwitchSceneButton", "SwitchScene")] [ValueDropdown("GetRepositionActionKeys")] [SerializeField]
        private string testRepositionActionKey;

        private void Start()
        {
            SetEventRegister();
            StartSwitchDefaultScene();
        }

        private void StartSwitchDefaultScene()
        {
            SceneRepositionSetting[] sceneRepositionSettings = sceneProcessSetting.GetSceneRepositionSettings();
            if (sceneRepositionSettings == null || sceneRepositionSettings.Length == 0)
                return;

            SwitchScene(sceneRepositionSettings[0].GetRepositionActionKey);
        }

        private string[] GetRepositionActionKeys()
        {
            if (sceneProcessSetting == null)
                return Array.Empty<string>();

            SceneRepositionSetting[] sceneRepositionSettings = sceneProcessSetting.GetSceneRepositionSettings();
            if (sceneRepositionSettings == null || sceneRepositionSettings.Length == 0)
                return Array.Empty<string>();

            return sceneRepositionSettings.Select(x => x.GetRepositionActionKey).ToArray();
        }

        private void SetEventRegister()
        {
            eventRegister.Unregister<SwitchSceneEvent>(OnSwitchScene);
            eventRegister.Register<SwitchSceneEvent>(OnSwitchScene);
        }

        private void EditorSwitchSceneButton()
        {
            SwitchScene(testRepositionActionKey);
        }

        private void SwitchScene(string repositionActionKey)
        {
            SceneRepositionSetting sceneRepositionSetting =
                sceneProcessSetting.GetSceneRepositionSettings().FirstOrDefault(x => x.GetRepositionActionKey == repositionActionKey);

            if (sceneRepositionSetting == null)
            {
                Debug.Log($"[SceneProcessManager] SwitchScene Failed : reposition action key '{repositionActionKey}' is not exist");
                return;
            }

            string unloadSceneName = sceneRepositionSetting.GetUnloadSceneName;
            if (string.IsNullOrEmpty(unloadSceneName) == false)
                SceneManager.UnloadSceneAsync(unloadSceneName);

            string loadSceneName = sceneRepositionSetting.GetLoadSceneName;
            if (string.IsNullOrEmpty(loadSceneName) == false)
                SceneManager.LoadScene(loadSceneName, LoadSceneMode.Additive);
        }

        private void OnSwitchScene(SwitchSceneEvent eventInfo)
        {
            SwitchScene(eventInfo.RepositionActionKey);
        }
    }
}