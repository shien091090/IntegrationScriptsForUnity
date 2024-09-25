namespace SNShien.Common.AssetTools
{
    public class LoadingProgress
    {
        public string CurrentLoadedAssetName { get; private set; }
        public int LoadedCount { get; private set; }
        public int TotalAssetCount { get; }

        public float GetCompletedPercent =>
            TotalAssetCount == 0 ?
                0 :
                (float)LoadedCount / TotalAssetCount;

        public string GetCompletedPercentText => $"{GetCompletedPercent * 100:0.0}%";

        public LoadingProgress(int totalLoadAssetCount)
        {
            LoadedCount = 0;
            TotalAssetCount = totalLoadAssetCount;
        }

        public void AddLoadedAsset(string assetName)
        {
            CurrentLoadedAssetName = assetName;
            LoadedCount++;
        }
    }
}