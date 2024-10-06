using SNShien.Common.TesterTools;

namespace SNShien.Common.ProcessTools
{
    public class SceneModelInitializer : ISceneModelInitializer
    {
        private const string DEBUGGER_KEY = "SceneModelInitializer";
        
        private readonly Debugger debugger;

        public SceneModelInitializer()
        {
            debugger = new Debugger(DEBUGGER_KEY);
        }

        public void ExecuteAllModel()
        {
            debugger.ShowLog("ExecuteAllModel");
        }
    }
}