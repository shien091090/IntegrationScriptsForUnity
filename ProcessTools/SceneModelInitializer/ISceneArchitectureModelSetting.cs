namespace SNShien.Common.ProcessTools
{
    public interface ISceneArchitectureModelSetting
    {
        string SceneName { get; }
        int GetModelOrder(string modelName);
    }
}