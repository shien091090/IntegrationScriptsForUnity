using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SNShien.Common.ProcessTools
{
    public class SceneProcessScriptableObject : SerializedScriptableObject, ISceneProcessSetting
    {
        public const string EMPTY_SCENE_NAME = "{Empty}";

        [SerializeField] [OnValueChanged("OnValueChanged")]
        private string[] sceneNameDefines;

        [SerializeField] [OnValueChanged("OnValueChanged")]
        private SceneRepositionSetting[] sceneRepositionSettings;

        public SceneRepositionSetting[] GetSceneRepositionSettings()
        {
            return sceneRepositionSettings;
        }

        private void OnValueChanged()
        {
            if (sceneRepositionSettings == null)
                return;

            List<string> sceneNameSelections = new List<string> { EMPTY_SCENE_NAME };
            sceneNameSelections.AddRange(sceneNameDefines);

            for (int index = 0; index < sceneRepositionSettings.Length; index++)
            {
                SceneRepositionSetting sceneRepositionSetting = sceneRepositionSettings[index];
                sceneRepositionSetting.SetSceneNames(sceneNameSelections);
            }
        }
    }
}