using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace SNShien.Common.AssetTools
{
    public class LoadAssetKey
    {
        public LoadAssetKeyType KeyType { get; }
        public string Key { get; }
        public IResourceLocation ResourceLocation { get; }

        public LoadAssetKey(string assetName)
        {
            Key = assetName;
            KeyType = LoadAssetKeyType.AssetName;
        }

        public LoadAssetKey(IResourceLocation resourceLocation)
        {
            ResourceLocation = resourceLocation;
            Key = resourceLocation.PrimaryKey;

            if (resourceLocation.ResourceType == typeof(SceneInstance))
                KeyType = LoadAssetKeyType.ResourceLocation_Scene;
            else
                KeyType = LoadAssetKeyType.ResourceLocation;
        }
    }
}