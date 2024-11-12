using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SNShien.Common.ProcessTools
{
    [CreateAssetMenu(fileName = "SceneProcessSetting", menuName = "SNShien/Create SceneProcessSetting")]
    public class SceneProcessScriptableObject : SerializedScriptableObject, ISceneProcessSetting
    {
        public const string EMPTY_SCENE_NAME = "{Empty}";

        [SerializeField] [OnValueChanged("OnValueChanged")] private SceneNameSetting[] sceneNameDefines;

        [SerializeField] [OnValueChanged("OnValueChanged")] private SceneRepositionSetting[] sceneRepositionSettings;

        public List<string> GetSceneNames =>
            sceneNameDefines == null || sceneNameDefines.Length == 0 ?
                new List<string>() :
                sceneNameDefines.Select(x => x.SceneName).Distinct().ToList();

        public SceneProcessSetting GetSceneProcessSetting()
        {
            return new SceneProcessSetting(sceneNameDefines, sceneRepositionSettings);
        }

        private void OnValueChanged()
        {
            if (sceneRepositionSettings == null)
                return;

            List<string> sceneNameSelections = new List<string> { EMPTY_SCENE_NAME };
            sceneNameSelections.AddRange(sceneNameDefines.Select(x => x.SceneName));

            for (int index = 0; index < sceneRepositionSettings.Length; index++)
            {
                SceneRepositionSetting sceneRepositionSetting = sceneRepositionSettings[index];
                sceneRepositionSetting.SetSceneNames(sceneNameSelections);
            }
        }
    }
}