using UnityEngine;

namespace SNShien.Common.ProcessTools
{
    public class SceneNameSetting
    {
        [SerializeField] private string sceneName;
        [SerializeField] private SceneResourceType sceneResourceType;

        public string SceneName => sceneName;
        public SceneResourceType GetSceneResourceType => sceneResourceType;
    }
}