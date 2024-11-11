using System.Collections.Generic;
using System.Linq;
using SNShien.Common.TesterTools;

namespace SNShien.Common.ProcessTools
{
    public class SceneModelInitializer : ISceneModelInitializer
    {
        private const string DEBUGGER_KEY = "SceneModelInitializer";

        private readonly ISceneArchitectureModelSetting sceneModelSetting;
        private readonly Debugger debugger = new Debugger(DEBUGGER_KEY);

        private List<IArchitectureModel> modelList = new List<IArchitectureModel>();

        public SceneModelInitializer(ISceneArchitectureModelSetting sceneModelSetting)
        {
            this.sceneModelSetting = sceneModelSetting;
        }

        public void ExecuteAllModel()
        {
            SortModel();
            PrintExecuteAllModelLog();

            foreach (IArchitectureModel model in modelList)
            {
                model.ExecuteModelInit();
            }
        }

        public void RegisterModel(IArchitectureModel model)
        {
            modelList.Add(model);
        }

        public void ReleaseAllModel()
        {
            debugger.ShowLog("release all model");
            
            foreach (IArchitectureModel model in modelList)
            {
                model.Release();
            }
            
            modelList.Clear();
        }

        private void PrintExecuteAllModelLog()
        {
            List<string> modelNameList = modelList.Select(x => x.GetType().Name).ToList();
            for (int i = 0; i < modelNameList.Count; i++)
            {
                string modelName = modelNameList[i];
                modelNameList[i] = $"{i + 1}. {modelName}";
            }

            string log = modelNameList.Count == 0 ?
                "{Empty}" :
                string.Join("\n", modelNameList);

            debugger.ShowLog(sceneModelSetting == null ?
                $"ExecuteAllModel, use model setting: false, model list count: {modelNameList.Count}, order list:\n{log}" :
                $"ExecuteAllModel, use model setting: true, current scene name: {sceneModelSetting.SceneName}, model list count: {modelNameList.Count}, order list:\n{log}");
        }

        private void SortModel()
        {
            if (sceneModelSetting == null)
                return;

            modelList = modelList
                .OrderBy(x => sceneModelSetting.GetModelOrder(x.GetType().Name))
                .ToList();
        }
    }
}