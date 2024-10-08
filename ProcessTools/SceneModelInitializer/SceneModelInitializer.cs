using System.Collections.Generic;
using System.Linq;
using SNShien.Common.TesterTools;
using Zenject;

namespace SNShien.Common.ProcessTools
{
    public class SceneModelInitializer : ISceneModelInitializer
    {
        private const string DEBUGGER_KEY = "SceneModelInitializer";

        [InjectOptional] private IArchitectureModelSetting modelSetting;

        private readonly Debugger debugger = new Debugger(DEBUGGER_KEY);

        private List<IArchitectureModel> modelList = new List<IArchitectureModel>();
        
        public void ExecuteAllModel()
        {
            SortModel();

            List<string> modelNameList = modelList.Select(x => x.GetType().Name).ToList();
            for (int i = 0; i < modelNameList.Count; i++)
            {
                string modelName = modelNameList[i];
                modelNameList[i] = $"{i + 1}. {modelName}";
            }

            string log = modelNameList.Count == 0 ?
                "{Empty}" :
                string.Join("\n", modelNameList);
            debugger.ShowLog($"ExecuteAllModel, model list count:{modelNameList.Count}, list:\n{log}");

            foreach (IArchitectureModel model in modelList)
            {
                model.ExecuteModelInit();
            }
        }

        public void RegisterModel(IArchitectureModel model)
        {
            modelList.Add(model);
        }

        private void SortModel()
        {
            if (modelSetting == null)
                return;

            modelList = modelList
                .OrderBy(x => modelSetting.GetModelOrder(x.GetType().Name))
                .ToList();
        }
    }
}