using UnityEngine.ResourceManagement.ResourceLocations;
using Object = UnityEngine.Object;

namespace SNShien.Common.AssetTools
{
    public class LoadingAssetResource
    {
        public LoadAssetKey LoadAssetKey { get; }
        public Object LoadedObjResult { get; private set; }
        public bool IsLoadedSuccess => LoadedObjResult != null;

        public LoadingAssetResource(string assetName)
        {
            LoadAssetKey = new LoadAssetKey(assetName);
        }

        public LoadingAssetResource(IResourceLocation resourceLocation)
        {
            LoadAssetKey = new LoadAssetKey(resourceLocation);
        }

        public void SetLoadedAsset(Object loadedObjResult)
        {
            LoadedObjResult = loadedObjResult;
        }

        public bool CompareLoadedObjectName(string loadedObjName)
        {
            return LoadAssetKey.Key.Contains(loadedObjName);
        }
    }
}