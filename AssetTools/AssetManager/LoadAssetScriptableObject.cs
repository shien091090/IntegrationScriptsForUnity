using UnityEngine;

namespace SNShien.Common.AssetTools
{
    public class LoadAssetScriptableObject : ScriptableObject, ILoadAssetSetting
    {
        [SerializeField] private string[] loadPrefabName;
        [SerializeField] private string[] loadScriptableObjectNames;
        public string[] GetLoadPrefabNames => loadPrefabName;
        public string[] GetLoadScriptableObjectNames => loadScriptableObjectNames;
    }
}