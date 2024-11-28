namespace GameCore
{
    public interface IMVPView
    {
        void BindPresenter(IMVPPresenter mvpPresenter);
        void UnbindPresenter();
    }
}