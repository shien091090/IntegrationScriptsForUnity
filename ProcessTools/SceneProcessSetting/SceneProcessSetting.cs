using System.Linq;

namespace SNShien.Common.ProcessTools
{
    public class SceneProcessSetting
    {
        private readonly SceneNameSetting[] sceneNameDefines;
        private readonly SceneRepositionSetting[] sceneRepositionSettings;

        public bool IsRepositionSettingEmpty => sceneRepositionSettings == null || sceneRepositionSettings.Length == 0;
        public string[] GetRepositionActionKeys => sceneRepositionSettings.Select(x => x.GetRepositionActionKey).ToArray();

        public string GetDefaultRepositionActionKey => IsRepositionSettingEmpty ?
            string.Empty :
            sceneRepositionSettings[0].GetLoadSceneName;

        public SceneProcessSetting(SceneNameSetting[] sceneNameDefines, SceneRepositionSetting[] sceneRepositionSettings)
        {
            this.sceneNameDefines = sceneNameDefines;
            this.sceneRepositionSettings = sceneRepositionSettings;
        }

        public SceneRepositionSetting GetRepositionSetting(string repositionActionKey)
        {
            return sceneRepositionSettings.FirstOrDefault(x => x.GetRepositionActionKey == repositionActionKey);
        }

        public SceneResourceType GetSceneResourceType(string loadSceneName)
        {
            SceneNameSetting sceneNameSetting = sceneNameDefines.FirstOrDefault(x => x.SceneName == loadSceneName);
            return sceneNameSetting?.GetSceneResourceType ?? SceneResourceType.Default;
        }
    }
}