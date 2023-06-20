namespace SNShien.Common.AssetTools
{
    public class LoadingProgress
    {
        public string AssetName { get; }
        public int LoadedCount { get; }
        public int TotalAssetCount { get; }

        public float GetCompletedPercent =>
            TotalAssetCount == 0 ?
                0 :
                (float)LoadedCount / TotalAssetCount;

        public string GetCompletedPercentText => $"{GetCompletedPercent * 100:0.0}%";

        public LoadingProgress(string assetName, int loadedCount, int totalAssetCount)
        {
            AssetName = assetName;
            LoadedCount = loadedCount;
            TotalAssetCount = totalAssetCount;
        }
    }
}