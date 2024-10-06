namespace SNShien.Common.ProcessTools
{
    public abstract class SceneInitializeInstaller : ZenjectGameObjectSpawner
    {
        public override void InstallBindings()
        {
            Container.Bind<ISceneModelInitializer>().To<SceneModelInitializer>().AsSingle();
            ExecuteInstaller();
        }

        protected abstract void ExecuteInstaller();

        private void Awake()
        {
            ISceneModelInitializer sceneModelInitializer = Container.Resolve<ISceneModelInitializer>();
            sceneModelInitializer.ExecuteAllModel();
        }
    }
}