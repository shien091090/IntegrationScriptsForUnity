using System;
using System.Linq;
using Sirenix.OdinInspector;
using SNShien.Common.TesterTools;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace SNShien.Common.ProcessTools
{
    public class SceneProcessManager : SerializedMonoBehaviour
    {
        private const string DEBUGGER_KEY = "SceneProcessManager";
        
        [Inject] private ISceneProcessSetting sceneProcessSetting;
        [Inject] private IEventRegister eventRegister;

        [InlineButton("EditorSwitchSceneButton", "SwitchScene")] [ValueDropdown("GetRepositionActionKeys")] [SerializeField]
        private string testRepositionActionKey;

        private Debugger debugger;

        public SceneProcessManager()
        {
            debugger = new Debugger(DEBUGGER_KEY);
        }

        private void Start()
        {
            SetEventRegister();
            StartSwitchDefaultScene();
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

        private void StartSwitchDefaultScene()
        {
            SceneRepositionSetting[] sceneRepositionSettings = sceneProcessSetting.GetSceneRepositionSettings();
            if (sceneRepositionSettings == null || sceneRepositionSettings.Length == 0)
                return;

            SwitchScene(sceneRepositionSettings[0].GetRepositionActionKey);
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
                debugger.ShowLog($"SwitchScene Failed, reposition action key '{repositionActionKey}' is not exist");
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