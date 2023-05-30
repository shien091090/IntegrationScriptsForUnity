namespace SNShien.Common.AssetTools
{
    public class LoadingAssetResource
    {
        public AssetResourceType ResourceType { get; }
        public string AssetName { get; }

        public static LoadingAssetResource CreatePrefabAsset(string loadPrefabName)
        {
            return new LoadingAssetResource(AssetResourceType.Prefab, loadPrefabName);
        }

        public static LoadingAssetResource CreateScriptableObjectAsset(string loadScriptableObjectName)
        {
            return new LoadingAssetResource(AssetResourceType.ScriptableObject, loadScriptableObjectName);
        }

        private LoadingAssetResource(AssetResourceType type, string assetName)
        {
            ResourceType = type;
            AssetName = assetName;
        }
    }
}