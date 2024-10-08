namespace SNShien.Common.MonoBehaviorTools
{
    public class InSceneViewInfo
    {
        public ViewState CurrentViewState { get; private set; }
        public IArchitectureView View { get; }

        public InSceneViewInfo(IArchitectureView view = null)
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