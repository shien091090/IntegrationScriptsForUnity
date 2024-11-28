using System.Collections.Generic;
using SNShien.Common.DataTools;
using SNShien.Common.TesterTools;

namespace GameCore
{
    public class DynamicMVPBinder
    {
        private const string DEBUGGER_KEY = "DynamicMVPBinder";

        private readonly Dictionary<int, (IMVPModel model, IMVPPresenter presenter, IMVPView view)> mvpDataDict =
            new Dictionary<int, (IMVPModel model, IMVPPresenter presenter, IMVPView view)>();

        private readonly Debugger debugger = new Debugger(DEBUGGER_KEY);

        public T GetPresenter<T>(IMVPModel mvpModel) where T : IMVPPresenter
        {
            if (mvpDataDict.TryGetValue(mvpModel.GetHashCode(), out (IMVPModel model, IMVPPresenter presenter, IMVPView view) mvpData))
            {
                if (mvpData.presenter is T presenter)
                    return presenter;
            }

            return default;
        }

        public void MultipleBind(IMVPModel mvpModel, IMVPPresenter mvpPresenter, IMVPView mvpView)
        {
            mvpView.BindPresenter(mvpPresenter);
            mvpPresenter.BindView(mvpView);

            mvpModel.BindPresenter(mvpPresenter);
            mvpPresenter.BindModel(mvpModel);

            mvpDataDict[mvpModel.GetHashCode()] = (mvpModel, mvpPresenter, mvpView);

            PrintCurrentDictStateLog();
        }

        public void RebindView(IMVPModel mvpModel, IMVPView newMvpView)
        {
            if (mvpDataDict.TryGetValue(mvpModel.GetHashCode(), out (IMVPModel model, IMVPPresenter presenter, IMVPView view) oldMvpData) == false)
                return;

            oldMvpData.presenter.UnbindView();
            oldMvpData.view.UnbindPresenter();

            oldMvpData.presenter.BindView(newMvpView);
            newMvpView.BindPresenter(oldMvpData.presenter);

            PrintCurrentDictStateLog();
        }

        private void PrintCurrentDictStateLog()
        {
            List<string> logList = new List<string>();
            foreach (KeyValuePair<int, (IMVPModel model, IMVPPresenter presenter, IMVPView view)> mvpDataPair in mvpDataDict)
            {
                string log =
                    $"[{mvpDataPair.Key}]: Model: {mvpDataPair.Value.model.GetHashCode()}, Presenter: {mvpDataPair.Value.presenter.GetHashCode()}, View: {mvpDataPair.Value.view.GetHashCode()}";
                logList.Add(log);
            }

            debugger.ShowLog(string.Join("\n", logList), true);
        }
    }
}