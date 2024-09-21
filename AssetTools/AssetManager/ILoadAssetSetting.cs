namespace SNShien.Common.AssetTools
{
    public interface ILoadAssetSetting
    {
        public string[] GetLoadAssetNames { get; }
        string[] GetLoadAssetLabels { get; }
        bool IsNeedLoadAssetByLabel { get; }
    }
}