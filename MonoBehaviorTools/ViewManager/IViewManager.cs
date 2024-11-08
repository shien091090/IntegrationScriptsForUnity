using SNShien.Common.ProcessTools;

namespace SNShien.Common.MonoBehaviorTools
{
    public interface IViewManager : IArchitectureModel
    {
        void ClearAllView();
        void OpenView<T>(params object[] parameters) where T : IArchitectureView;
    }
}