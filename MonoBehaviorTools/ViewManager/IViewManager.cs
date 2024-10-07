namespace SNShien.Common.MonoBehaviorTools
{
    public interface IViewManager
    {
        void OpenView<T>(params object[] parameters) where T : IArchitectureView;
    }
}