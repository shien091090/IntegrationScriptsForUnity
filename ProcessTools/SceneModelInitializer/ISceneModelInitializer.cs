using GameCore;

namespace SNShien.Common.ProcessTools
{
    public interface ISceneModelInitializer
    {
        void ExecuteAllModel();
        void RegisterModel(IArchitectureModel model);
    }
}