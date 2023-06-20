using System;
using Object = UnityEngine.Object;

namespace SNShien.Common.AssetTools
{
    public interface IAssetManager
    {
        T GetAsset<T>(string assetName) where T : Object;
        void StartLoadAsset();
        event Action OnAllAssetLoadCompleted;
        event Action<LoadingProgress> OnUpdateLoadingProgress;
    }
}