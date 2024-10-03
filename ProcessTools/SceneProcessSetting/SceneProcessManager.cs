using System;
using System.Linq;
using Sirenix.OdinInspector;
using SNShien.Common.TesterTools;
using UnityEngine;
using UnityEngine.AddressableAssets;
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

            SceneProcessSetting processSetting = sceneProcessSetting.GetSceneProcessSetting();
            return processSetting.IsRepositionSettingEmpty ?
                Array.Empty<string>() :
                processSetting.GetRepositionActionKeys;
        }

        private void SetEventRegister()
        {
            eventRegister.Unregister<SwitchSceneEvent>(OnSwitchScene);
            eventRegister.Register<SwitchSceneEvent>(OnSwitchScene);
        }

        private void StartSwitchDefaultScene()
        {
            string defaultRepositionActionKey = sceneProcessSetting.GetSceneProcessSetting().GetDefaultRepositionActionKey;
            if (string.IsNullOrEmpty(defaultRepositionActionKey) == false)
                SwitchScene(defaultRepositionActionKey);
        }

        private void EditorSwitchSceneButton()
        {
            SwitchScene(testRepositionActionKey);
        }

        private void SwitchScene(string repositionActionKey)
        {
            SceneRepositionSetting sceneRepositionSetting = sceneProcessSetting.GetSceneProcessSetting().GetRepositionSetting(repositionActionKey);

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
            {
                SceneResourceType sceneResourceType = sceneProcessSetting.GetSceneProcessSetting().GetSceneResourceType(loadSceneName);
                switch (sceneResourceType)
                {
                    case SceneResourceType.FromAddressableBundle:
                        Addressables.LoadSceneAsync(loadSceneName, LoadSceneMode.Additive);
                        break;

                    default:
                        SceneManager.LoadScene(loadSceneName, LoadSceneMode.Additive);
                        break;
                }
            }
        }

        private void OnSwitchScene(SwitchSceneEvent eventInfo)
        {
            SwitchScene(eventInfo.RepositionActionKey);
        }
    }
}