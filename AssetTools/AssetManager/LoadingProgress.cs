using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;

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

        public void AddLoadedAsset(object assetObj)
        {
            CurrentLoadedAssetName = ConvertAssetName(assetObj);
            LoadedCount++;
        }

        private string ConvertAssetName(object assetObj)
        {
            if (assetObj is Object unityObj)
                return unityObj.name;
            else if (assetObj is SceneInstance sceneInstance)
                return sceneInstance.Scene.name;
            else if (assetObj is string)
                return (string)assetObj; 
            else
                return assetObj.GetType().Name;
        }
    }
}