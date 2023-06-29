namespace SNShien.Common.AssetTools
{
    public interface ILoadAssetSetting
    {
        public string[] GetLoadPrefabNames { get; }
        public string[] GetLoadScriptableObjectNames { get; }
        public string[] GetLoadOtherAssetNames { get; }
    }
}