using SNShien.Common.TesterTools;

namespace SNShien.Common.ProcessTools
{
    public abstract class SceneInitializeInstaller : ZenjectGameObjectSpawner
    {
        private const string DEBUGGER_KEY = "SceneInitializeInstaller";

        private ISceneModelInitializer sceneModelInitializer;
        private Debugger debugger;

        public override void InstallBindings()
        {
            debugger = new Debugger(DEBUGGER_KEY);

            Container.Bind<ISceneModelInitializer>().To<SceneModelInitializer>().AsSingle();
            sceneModelInitializer = Container.Resolve<ISceneModelInitializer>();
            ExecuteInstaller();
        }

        private bool CheckAddInitModelList<T>()
        {
            T resolve = Container.Resolve<T>();
            if (resolve is IArchitectureModel model)
            {
                sceneModelInitializer.RegisterModel(model);
                return true;
            }
            else
                return false;
        }

        protected abstract void ExecuteInstaller();

        private void Awake()
        {
            sceneModelInitializer.ExecuteAllModel();
        }

        protected void BindModel<T1, T2>() where T2 : T1
        {
            Container.Bind<T1>().To<T2>().AsSingle();
            debugger.ShowLog(CheckAddInitModelList<T1>() ?
                $"BindModel Success, model:{typeof(T2).Name}" :
                $"BindModel Failed, model:{typeof(T2).Name}");
        }

        protected void BindModelFromInstance<T1, T2>(T2 instance) where T2 : T1
        {
            Container.Bind<T1>().FromInstance(instance).AsSingle();
            debugger.ShowLog(CheckAddInitModelList<T1>() ?
                $"BindModelFromInstance Success, model:{typeof(T2).Name}" :
                $"BindModelFromInstance Failed, model:{typeof(T2).Name}");
        }
    }
}