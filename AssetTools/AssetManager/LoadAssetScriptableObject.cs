using UnityEngine;

namespace SNShien.Common.AssetTools
{
    public class LoadAssetScriptableObject : ScriptableObject, ILoadAssetSetting
    {
        [SerializeField] private string[] loadAssetNames;
        [SerializeField] private string[] loadAssetLabels;

        public string[] GetLoadAssetNames => loadAssetNames;
        public string[] GetLoadAssetLabels => loadAssetLabels;
        public bool IsNeedLoadAssetByLabel => GetLoadAssetLabels != null && GetLoadAssetLabels.Length > 0;
    }
}