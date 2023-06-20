using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using Zenject;
using Object = UnityEngine.Object;

namespace SNShien.Common.AssetTools
{
    public class LoadAssetManager : IAssetManager
    {
        [Inject] private ILoadAssetSetting loadAssetSetting;

        private readonly Dictionary<string, Object> assetDict = new Dictionary<string, Object>();
        private Queue<LoadingAssetResource> loadAssetQueue = new Queue<LoadingAssetResource>();
        private LoadingAssetResource currentAssetInfo;
        private int totalAssetCount;

        public event Action<LoadingProgress> OnUpdateLoadingProgress;
        public event Action OnAllAssetLoadCompleted;

        public T GetAsset<T>(string assetName) where T : Object
        {
            if (!assetDict.ContainsKey(assetName))
                return default;

            Object obj = assetDict[assetName];
            if (obj is T result)
                return result;

            if (!(obj is GameObject go))
                return default;

            T component = go.GetComponent<T>();
            return component != null ?
                component :
                default;
        }

        public void StartLoadAsset()
        {
            loadAssetQueue = new Queue<LoadingAssetResource>();
            foreach (string loadPrefabName in loadAssetSetting.GetLoadPrefabNames)
            {
                loadAssetQueue.Enqueue(LoadingAssetResource.CreatePrefabAsset(loadPrefabName));
            }

            foreach (string loadScriptableObjectName in loadAssetSetting.GetLoadScriptableObjectNames)
            {
                loadAssetQueue.Enqueue(LoadingAssetResource.CreateScriptableObjectAsset(loadScriptableObjectName));
            }

            totalAssetCount = loadAssetQueue.Count;
            OnUpdateLoadingProgress?.Invoke(new LoadingProgress(string.Empty, 0, totalAssetCount));
            LoadAssetQueue(loadAssetQueue);
        }

        private void LoadAssetQueue(Queue<LoadingAssetResource> assetNameQueue)
        {
            if (assetNameQueue == null || assetNameQueue.Count == 0)
            {
                AllAssetLoadCompleted();
                return;
            }

            LoadingAssetResource assetInfo = assetNameQueue.Dequeue();
            currentAssetInfo = assetInfo;

            switch (currentAssetInfo.ResourceType)
            {
                case AssetResourceType.Prefab:
                    Addressables.LoadAssetAsync<GameObject>(currentAssetInfo.AssetName).Completed += LoadAssetCompleted;
                    break;

                case AssetResourceType.ScriptableObject:
                    Addressables.LoadAssetAsync<ScriptableObject>(currentAssetInfo.AssetName).Completed += LoadAssetCompleted;
                    break;

                default:
                    LoadAssetQueue(loadAssetQueue);
                    break;
            }
        }

        private void LoadAssetCompleted<T>(AsyncOperationHandle<T> loadedObj) where T : Object
        {
            Debug.Log($"[AssetManager] LoadAssetCompleted, Name = {loadedObj.Result.name}, Status = {loadedObj.Status}");

            if (loadedObj.Status == AsyncOperationStatus.Succeeded)
                assetDict[loadedObj.Result.name] = loadedObj.Result;

            OnUpdateLoadingProgress?.Invoke(new LoadingProgress(loadedObj.Result.name, assetDict.Count, totalAssetCount));
            currentAssetInfo = null;
            LoadAssetQueue(loadAssetQueue);
        }

        private void AllAssetLoadCompleted()
        {
            // PrintLog();
            OnAllAssetLoadCompleted?.Invoke();
        }

        private void PrintLog()
        {
            IList<IResourceProvider> providers = Addressables.ResourceManager.ResourceProviders;
            Debug.Log($"providers.Count = {providers.Count}");
            foreach (IResourceProvider provider in providers)
            {
                Debug.Log($"ProviderId = {provider.ProviderId}");
            }
        }
    }
}