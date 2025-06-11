using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SNShien.Common.TesterTools;
using UnityEditor;
using UnityEngine;

public class ScriptingDefineSettingWindow : EditorWindow
{
    private const string LOCAL_FOLDER_PATH = "Assets/LocalFiles/";
    private const string LOCAL_FILE_NAME = "ScriptingDefineSetting.txt";

    private static ScriptingDefineSettingWindow eidotrWindow;
    private static Dictionary<string, bool> scriptingDefineSelectStateDict;
    private static List<string> scriptingDefineOptions;

    private readonly Debugger debugger = new Debugger("ScriptingDefineSettingWindow");

    [MenuItem("SNTool/ScriptingDefineSetting")]
    private static void Init()
    {
        InitWindow();
        InitDropdownState();
    }

    private static void InitWindow()
    {
        eidotrWindow = GetWindow<ScriptingDefineSettingWindow>(true, "ScriptingDefineSetting");
        eidotrWindow.minSize = new Vector2(250, 160);
        eidotrWindow.Show();
    }

    private static List<string> GetScriptingDefineOptions()
    {
        if (scriptingDefineOptions == null || scriptingDefineOptions.Count == 0)
        {
            scriptingDefineOptions = new List<string>
            {
                "CUSTOM_USING_ZENJECT",
                "CUSTOM_USING_ADDRESSABLE",
                "CUSTOM_USING_FMOD",
                "CUSTOM_USING_ODIN",
                "CUSTOM_USING_DOTWEEN"
            };
        }

        return scriptingDefineOptions;
    }

    private static void InitDropdownState()
    {
        scriptingDefineSelectStateDict ??= new Dictionary<string, bool>();

        List<string> loadFileDefineList = ConvertLoadFileToDefineList();
        foreach (string opt in GetScriptingDefineOptions())
        {
            scriptingDefineSelectStateDict[opt] = loadFileDefineList.Contains(opt);
        }
    }

    private static List<string> ConvertLoadFileToDefineList()
    {
        List<string> result = new List<string>();
        List<string> loadFile = LoadFile();
        if (loadFile.Count == 0)
            return result;

        foreach (string loadLine in loadFile)
        {
            string[] loadLineSplit = loadLine.Split(';');
            if (loadLineSplit.Length > 0)
                result.AddRange(loadLineSplit);
        }

        result = result.Distinct().ToList();
        return result;
    }

    private static List<string> LoadFile()
    {
        if (File.Exists(GetFullLocalFilePath()) == false)
            return new List<string>();

        List<string> result = new List<string>();
        StreamReader reader = new StreamReader(GetFullLocalFilePath(), Encoding.GetEncoding("utf-8"));

        string line = string.Empty;
        while ((line = reader.ReadLine()) != null)
        {
            result.Add(line);
        }

        reader.Close();
        return result;
    }

    private static string GetFullLocalFilePath()
    {
        return $"{LOCAL_FOLDER_PATH}{LOCAL_FILE_NAME}";
    }

    private List<string> GetSelectingOptionList()
    {
        List<string> result = new List<string>();

        foreach (KeyValuePair<string, bool> pair in GetScriptingDefineSelectStateDict())
        {
            if (pair.Value)
                result.Add(pair.Key);
        }

        return result;
    }

    private Dictionary<string, bool> GetScriptingDefineSelectStateDict()
    {
        if (scriptingDefineSelectStateDict == null || scriptingDefineSelectStateDict.Count == 0)
            InitDropdownState();

        return scriptingDefineSelectStateDict;
    }

    private void SaveFile(string saveString)
    {
        if (File.Exists(LOCAL_FOLDER_PATH) == false)
            Directory.CreateDirectory(Path.GetDirectoryName(LOCAL_FOLDER_PATH));

        if (File.Exists(GetFullLocalFilePath()))
            File.Delete(GetFullLocalFilePath());

        StreamWriter writer = new StreamWriter(GetFullLocalFilePath(), false, Encoding.GetEncoding("utf-8"));
        writer.WriteLine(saveString);
        writer.Close();
    }

    private void ShowMultiSelectionPanel()
    {
        GUILayout.Label("ScriptingDefines", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical("box");
        {
            Dictionary<string, bool> defineSelectStateDict = GetScriptingDefineSelectStateDict();
            foreach (string opt in GetScriptingDefineOptions())
            {
                defineSelectStateDict[opt] = EditorGUILayout.ToggleLeft(opt, defineSelectStateDict[opt]);
            }
        }
        EditorGUILayout.EndVertical();
    }

    private void ShowApplyButton()
    {
        if (GUILayout.Button("套用") == false)
            return;

        BuildTargetGroup targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        string currentDefinesJoin = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);

        List<string> newDefineList = new List<string>();
        if (currentDefinesJoin != null && currentDefinesJoin.Length > 0)
        {
            List<string> options = GetScriptingDefineOptions();
            foreach (string currentDefine in currentDefinesJoin.Split(';'))
            {
                bool isMatchAny = options.Any(option => currentDefine == option);
                if (isMatchAny == false)
                    newDefineList.Add(currentDefine);
            }
        }

        newDefineList.AddRange(GetSelectingOptionList());
        newDefineList = newDefineList.Distinct().ToList();
        string newDefinesJoin = string.Join(";", newDefineList);
        SaveFile(newDefinesJoin);
        debugger.ShowLog($"newDefinesJoin: {newDefinesJoin}");

        PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, newDefinesJoin);
    }

    private void OnGUI()
    {
        ShowMultiSelectionPanel();
        ShowApplyButton();
    }
}