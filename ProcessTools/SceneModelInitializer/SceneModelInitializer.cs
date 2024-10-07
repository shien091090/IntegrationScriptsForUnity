using System.Collections.Generic;
using System.Linq;
using SNShien.Common.TesterTools;

namespace SNShien.Common.ProcessTools
{
    public class SceneModelInitializer : ISceneModelInitializer
    {
        private const string DEBUGGER_KEY = "SceneModelInitializer";

        private readonly Debugger debugger;
        private readonly List<IArchitectureModel> modelList = new List<IArchitectureModel>();

        public SceneModelInitializer()
        {
            debugger = new Debugger(DEBUGGER_KEY);
        }

        public void ExecuteAllModel()
        {
            List<string> modelNameList = modelList.Select(x => x.GetType().Name).ToList();
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
    }
}