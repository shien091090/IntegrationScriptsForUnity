using System;
using System.Collections.Generic;
using System.Linq;
using SNShien.Common.TesterTools;
using UnityEngine;
using Zenject;

namespace SNShien.Common.ProcessTools
{
    public abstract class SceneInitializeInstaller : MonoInstaller
    {
        private const string DEBUGGER_KEY = "SceneInitializeInstaller";

        [InjectOptional] private IArchitectureModelSetting modelSetting;
        [InjectOptional] private ISceneProcessManager sceneProcessManager;

        private ISceneModelInitializer sceneModelInitializer;
        private Debugger debugger;
        private List<(Type modelType, Type modleInterfaceType)> waitForInitTypeList = new List<(Type modelType, Type modleInterfaceType)>();

        public event Action OnPreStartInitModel;

        public override void InstallBindings()
        {
            debugger = new Debugger(DEBUGGER_KEY);
            sceneModelInitializer = new SceneModelInitializer(GetCurrentSceneModelSetting());
            ExecuteInstaller();
        }

        private void Awake()
        {
            OnPreStartInitModel?.Invoke();

            InitModels();
            sceneModelInitializer.ExecuteAllModel();
        }

        private void OnDestroy()
        {
            sceneModelInitializer.ReleaseAllModel();
        }

        private void InitModels()
        {
            string currentSceneName = sceneProcessManager == null ?
                "{None}" :
                sceneProcessManager.CurrentMainScene;

            debugger.ShowLog($"Start init models, current scene name: {currentSceneName}", fontColor: Color.green);

            ISceneArchitectureModelSetting modelSetting = GetCurrentSceneModelSetting();
            if (modelSetting != null)
                waitForInitTypeList = waitForInitTypeList
                    .OrderBy(x => modelSetting.GetModelOrder(x.modelType.Name))
                    .ToList();

            foreach ((Type modelType, Type modelInterfaceType) in waitForInitTypeList)
            {
                debugger.ShowLog(CheckAddInitModelList(modelInterfaceType) ?
                    $"init model success, model:{modelType.Name}" :
                    $"init model failed, model:{modelType.Name}");
            }

            waitForInitTypeList.Clear();
        }

        private ISceneArchitectureModelSetting GetCurrentSceneModelSetting()
        {
            if (sceneProcessManager == null ||
                modelSetting == null)
                return null;

            string sceneName = sceneProcessManager.CurrentMainScene;
            return modelSetting.GetModelSetting(sceneName);
        }

        private bool CheckAddInitModelList(Type interfaceType)
        {
            object resolve = Container.Resolve(interfaceType);
            if (resolve is IArchitectureModel model)
            {
                sceneModelInitializer.RegisterModel(model);
                return true;
            }
            else
                return false;
        }

        protected abstract void ExecuteInstaller();

        protected void BindModel<T1, T2>() where T2 : T1
        {
            Container.Bind<T1>().To<T2>().AsSingle();
            waitForInitTypeList.Add((typeof(T2), typeof(T1)));
        }

        protected void BindModelFromInstance<T1, T2>(T2 instance) where T2 : T1
        {
            Container.Bind<T1>().FromInstance(instance).AsSingle();
            waitForInitTypeList.Add((typeof(T2), typeof(T1)));
        }
    }
}