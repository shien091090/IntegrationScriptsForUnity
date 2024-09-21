using UnityEngine.ResourceManagement.ResourceLocations;

namespace SNShien.Common.AssetTools
{
    public class LoadingAssetResource
    {
        public string AssetName { get; }
        public IResourceLocation ResourceLocation { get; }

        public LoadingAssetResource(string assetName)
        {
            AssetName = assetName;
        }

        public LoadingAssetResource(IResourceLocation resourceLocation)
        {
            ResourceLocation = resourceLocation;
        }
    }
}