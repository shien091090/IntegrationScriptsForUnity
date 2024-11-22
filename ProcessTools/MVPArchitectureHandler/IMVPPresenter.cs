namespace GameCore
{
    public interface IMVPPresenter
    {
        void BindModel(IMVPModel model);
        void BindView(IMVPView mvpView);
    }
}