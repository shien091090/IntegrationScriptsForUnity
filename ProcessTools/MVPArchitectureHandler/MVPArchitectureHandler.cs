namespace GameCore
{
    public class MVPArchitectureHandler : IMVPArchitectureHandler
    {
        public void MultipleBind(IMVPModel mvpModel, IMVPPresenter mvpPresenter, IMVPView mvpView)
        {
            mvpView.BindPresenter(mvpPresenter);
            mvpPresenter.BindView(mvpView);

            mvpModel.BindPresenter(mvpPresenter);
            mvpPresenter.BindModel(mvpModel);
        }
    }
}