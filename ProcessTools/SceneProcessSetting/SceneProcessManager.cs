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
    public class SceneProcessManager : SerializedMonoBehaviour, ISceneProcessManager
    {
        private const string DEBUGGER_KEY = "SceneProcessManager";

        [Inject] private ISceneProcessSetting sceneProcessSetting;
        [Inject] private IEventRegister eventRegister;

        [InlineButton("EditorSwitchSceneButton", "SwitchScene")] [ValueDropdown("GetRepositionActionKeys")] [SerializeField]
        private string testRepositionActionKey;

        public string CurrentMainScene { get; private set; }

        private Debugger debugger;
        private bool isAlreadyInit;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            if (isAlreadyInit)
                return;

            debugger = new Debugger(DEBUGGER_KEY);

            SetEventRegister();
            StartSwitchDefaultScene();

            isAlreadyInit = true;
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
            {
                debugger.ShowLog($"default reposition action key: {defaultRepositionActionKey}", true);
                SwitchScene(defaultRepositionActionKey);
            }
            else
                debugger.ShowLog("default reposition action key is empty", true);
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
                SceneProcessSetting processSetting = sceneProcessSetting.GetSceneProcessSetting();
                if (processSetting.IsFromBundle(loadSceneName, out string sceneBundlePath))
                    Addressables.LoadSceneAsync(sceneBundlePath, LoadSceneMode.Additive);
                else
                    SceneManager.LoadScene(loadSceneName, LoadSceneMode.Additive);

                CurrentMainScene = loadSceneName;
            }
        }

        private void OnSwitchScene(SwitchSceneEvent eventInfo)
        {
            SwitchScene(eventInfo.RepositionActionKey);
        }
    }
}