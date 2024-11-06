using System.Diagnostics;
using UnityEngine;
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

        public void ShowLog(string log, bool printPrefixMethodName = false, Color fontColor = default)
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

            if (fontColor == default)
                Debug.Log($"[{DebuggerKey}] {finalLog}");
            else
            {
                string colorHex = ColorUtility.ToHtmlStringRGB(fontColor);
                Debug.Log($"<color=#{colorHex}>[{DebuggerKey}] {finalLog}</color>");
            }
        }
    }
}