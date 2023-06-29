using UnityEngine;

namespace SNShien.Common.AssetTools
{
    public class LoadAssetScriptableObject : ScriptableObject, ILoadAssetSetting
    {
        [SerializeField] private string[] loadPrefabName;
        [SerializeField] private string[] loadScriptableObjectNames;
        [SerializeField] private string[] loadOtherAssetNames;

        public string[] GetLoadPrefabNames => loadPrefabName;
        public string[] GetLoadScriptableObjectNames => loadScriptableObjectNames;
        public string[] GetLoadOtherAssetNames => loadOtherAssetNames;
    }
}