#if CUSTOM_USING_ODIN
using Sirenix.OdinInspector;
using UnityEngine;

namespace SNShien.Common.ProcessTools
{
    public class SceneNameSetting
    {
        [SerializeField] private string sceneName;

        [ShowIf("sceneResourceType", SceneResourceType.FromAddressableBundle)] [SerializeField]
        private string sceneBundlePath;

        [SerializeField] private SceneResourceType sceneResourceType;

        public string SceneName => sceneName;
        public SceneResourceType GetSceneResourceType => sceneResourceType;
        public string GetSceneBundlePath => sceneBundlePath;
    }
}
#endif