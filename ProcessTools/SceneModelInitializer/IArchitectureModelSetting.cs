namespace SNShien.Common.ProcessTools
{
    public interface IArchitectureModelSetting
    {
        ISceneArchitectureModelSetting GetModelSetting(string sceneName);
    }
}