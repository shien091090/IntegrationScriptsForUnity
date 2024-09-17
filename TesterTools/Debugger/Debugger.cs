using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace SNShien.Common.TesterTools
{
    public class Debugger
    {
        private StackTrace stackTrace;
        public string DebuggerKey { get; private set; }

        public Debugger(string debuggerKey)
        {
            DebuggerKey = debuggerKey;
        }

        public void ShowLog(string log, bool printPrefixMethodName = false)
        {
            stackTrace = new StackTrace();

            string finalLog = string.Empty;
            if (printPrefixMethodName)
            {
                finalLog = stackTrace.GetFrame(1)?.GetMethod().Name;
                finalLog = $"[{finalLog}] {log}";
            }
            else
            {
                finalLog = log;
            }

            Debug.Log($"[{DebuggerKey}] {finalLog}");
        }
    }
}