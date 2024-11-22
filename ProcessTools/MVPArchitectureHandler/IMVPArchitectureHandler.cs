namespace GameCore
{
    public interface IMVPArchitectureHandler
    {
        void MultipleBind(IMVPModel mvpModel, IMVPPresenter mvpPresenter, IMVPView mvpView);
    }
}