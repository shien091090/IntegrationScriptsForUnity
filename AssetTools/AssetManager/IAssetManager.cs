using System;
using Object = UnityEngine.Object;

namespace SNShien.Common.AssetTools
{
    public interface IAssetManager
    {
        event Action OnAllAssetLoadCompleted;
        event Action<LoadingProgress> OnUpdateLoadingProgress;
        event Action<AssetLoadingState> OnUpdateLoadingState;
        T GetAsset<T>(string assetName);
        void StartPrecedingProcedures();
    }
}