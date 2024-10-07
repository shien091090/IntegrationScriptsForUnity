namespace SNShien.Common.MonoBehaviorTools
{
    public class InSceneViewInfo
    {
        public ViewState CurrentViewState { get; }

        public InSceneViewInfo()
        {
            CurrentViewState = ViewState.NotExist;
        }
    }
}