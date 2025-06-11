#if CUSTOM_USING_ADDRESSABLE
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace SNShien.Common.AssetTools
{
    public class LoadingAssetResource
    {
        public LoadAssetKey LoadAssetKey { get; }
        public object LoadedObjResult { get; private set; }
        public bool IsLoadedSuccess => LoadedObjResult != null;

        public LoadingAssetResource(string assetName)
        {
            LoadAssetKey = new LoadAssetKey(assetName);
        }

        public LoadingAssetResource(IResourceLocation resourceLocation)
        {
            LoadAssetKey = new LoadAssetKey(resourceLocation);
        }

        public void SetLoadedAsset(object loadedObjResult)
        {
            LoadedObjResult = loadedObjResult;
        }

        public bool CompareLoadedObjectName(string loadedObjName)
        {
            return LoadAssetKey.Key.Contains(loadedObjName);
        }
    }
}
#endif
