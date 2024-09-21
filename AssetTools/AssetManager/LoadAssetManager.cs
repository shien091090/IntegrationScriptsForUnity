using System;
using System.Collections.Generic;
using System.Linq;
using SNShien.Common.TesterTools;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using Zenject;
using Object = UnityEngine.Object;

namespace SNShien.Common.AssetTools
{
    public class LoadAssetManager : IAssetManager
    {
        private const string DEBUGGER_KEY = "LoadAssetManager";

        [Inject] private ILoadAssetSetting loadAssetSetting;

        private readonly Dictionary<string, Object> assetDict = new Dictionary<string, Object>();
        private readonly Debugger debugger;

        private Queue<LoadingAssetResource> loadAssetQueue = new Queue<LoadingAssetResource>();
        private Queue<string> assetLabelQueue = new Queue<string>();
        private LoadingAssetResource currentAssetInfo;
        private int totalAssetCount;

        public LoadAssetManager()
        {
            debugger = new Debugger(DEBUGGER_KEY);
        }

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
            ClearTempData();

            foreach (string assetName in loadAssetSetting.GetLoadAssetNames)
            {
                loadAssetQueue.Enqueue(new LoadingAssetResource(assetName));
            }

            if (loadAssetSetting.IsNeedLoadAssetByLabel)
            {
                assetLabelQueue = new Queue<string>(loadAssetSetting.GetLoadAssetLabels.ToList());
                LoadResourceLocationQueue(assetLabelQueue);
            }
            else
                StartLoadAssetQueue();
        }


        private void ClearTempData()
        {
            loadAssetQueue = new Queue<LoadingAssetResource>();
            assetLabelQueue = new Queue<string>();
            currentAssetInfo = null;
            totalAssetCount = 0;
        }

        private void StartLoadAssetQueue()
        {
            totalAssetCount = loadAssetQueue.Count;
            debugger.ShowLog($"totalAssetCount: {totalAssetCount}", true);

            OnUpdateLoadingProgress?.Invoke(new LoadingProgress(string.Empty, 0, totalAssetCount));
            LoadAssetQueue(loadAssetQueue);
        }

        private void LoadResourceLocationQueue(Queue<string> labelQueue)
        {
            if (labelQueue == null || labelQueue.Count == 0)
            {
                StartLoadAssetQueue();
                return;
            }

            string label = labelQueue.Dequeue();
            Addressables.LoadResourceLocationsAsync(label).Completed += OnLoadResourceLocationCompleted;
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

            if (string.IsNullOrEmpty(assetInfo.AssetName) == false)
                Addressables.LoadAssetAsync<Object>(currentAssetInfo.AssetName).Completed += OnLoadAssetCompleted;
            else
                Addressables.LoadAssetAsync<Object>(currentAssetInfo.ResourceLocation).Completed += OnLoadAssetCompleted;
        }

        private void AllAssetLoadCompleted()
        {
            // PrintLog();
            ClearTempData();
            OnAllAssetLoadCompleted?.Invoke();
        }

        private void PrintLog()
        {
            IList<IResourceProvider> providers = Addressables.ResourceManager.ResourceProviders;
            debugger.ShowLog($"providers.Count: {providers.Count}");
            foreach (IResourceProvider provider in providers)
            {
                debugger.ShowLog($"ProviderId: {provider.ProviderId}");
            }
        }

        private void OnLoadResourceLocationCompleted(AsyncOperationHandle<IList<IResourceLocation>> loadedObj)
        {
            Dictionary<string, IResourceLocation> resourceLocationDict = new Dictionary<string, IResourceLocation>();
            foreach (IResourceLocation resourceLocation in loadedObj.Result)
            {
                if (resourceLocationDict.ContainsKey(resourceLocation.PrimaryKey))
                {
                    if (resourceLocation.ResourceType.IsSubclassOf(typeof(GameObject)) ||
                        resourceLocation.ResourceType.IsSubclassOf(typeof(ScriptableObject)) ||
                        resourceLocation.ResourceType.IsSubclassOf(typeof(TextAsset)))
                        resourceLocationDict[resourceLocation.PrimaryKey] = resourceLocation;
                }
                else
                    resourceLocationDict[resourceLocation.PrimaryKey] = resourceLocation;
            }

            List<string> logs = resourceLocationDict.Values
                .Select(resourceLocation => $"PrimaryKey: {resourceLocation.PrimaryKey}, ResourceType: {resourceLocation.ResourceType}")
                .ToList();

            debugger.ShowLog($"ResourceLocation logs:\n{string.Join("\n", logs)}", true);

            foreach (IResourceLocation resourceLocation in resourceLocationDict.Values)
            {
                loadAssetQueue.Enqueue(new LoadingAssetResource(resourceLocation));
            }

            LoadResourceLocationQueue(assetLabelQueue);
        }

        private void OnLoadAssetCompleted<T>(AsyncOperationHandle<T> loadedObj) where T : Object
        {
            debugger.ShowLog($"Name: {loadedObj.Result.name}, Status: {loadedObj.Status}", true);

            if (loadedObj.Status == AsyncOperationStatus.Succeeded)
                assetDict[loadedObj.Result.name] = loadedObj.Result;

            OnUpdateLoadingProgress?.Invoke(new LoadingProgress(loadedObj.Result.name, assetDict.Count, totalAssetCount));
            currentAssetInfo = null;
            LoadAssetQueue(loadAssetQueue);
        }
    }
}