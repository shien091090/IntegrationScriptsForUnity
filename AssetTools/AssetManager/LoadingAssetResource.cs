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

        public static LoadingAssetResource CreateOtherAsset(string assetName)
        {
            return new LoadingAssetResource(AssetResourceType.Bytes, assetName);
        }

        private LoadingAssetResource(AssetResourceType type, string assetName)
        {
            ResourceType = type;
            AssetName = assetName;
        }
    }
}