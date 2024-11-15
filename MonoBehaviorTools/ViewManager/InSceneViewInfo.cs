namespace SNShien.Common.MonoBehaviorTools
{
    public class InSceneViewInfo
    {
        public ViewState CurrentViewState { get; private set; }
        public ArchitectureView View { get; }

        public InSceneViewInfo(ArchitectureView view = null)
        {
            CurrentViewState = ViewState.NotExist;
            View = view;
        }

        public void SetState(ViewState state)
        {
            CurrentViewState = state;
        }
    }
}