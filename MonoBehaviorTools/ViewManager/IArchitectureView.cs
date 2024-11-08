namespace SNShien.Common.MonoBehaviorTools
{
    public interface IArchitectureView
    {
        void UpdateView();
        void OpenView(params object[] parameters);
        void ReOpenView(params object[] parameters);
        void CloseView();
    }
}