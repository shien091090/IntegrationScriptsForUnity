#if CUSTOM_USING_ADDRESSABLE
using System;
using System.Collections.Generic;
using SNShien.Common.TesterTools;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Object = UnityEngine.Object;

namespace SNShien.Common.AssetTools
{
    public class LoadAssetProcess
    {
        private readonly Debugger debugger;

        private readonly Dictionary<string, IResourceLocation> resourceLocationDict = new Dictionary<string, IResourceLocation>();
        private readonly Dictionary<string, LoadingAssetResource> loadAssetResultDict = new Dictionary<string, LoadingAssetResource>();

        private Queue<string> loadResourceLocationLabelQueue = new Queue<string>();
        private Queue<LoadingAssetResource> loadAssetResourceQueue = new Queue<LoadingAssetResource>();
        private LoadingAssetResource currentLoadAsset;
        public bool IsNeedLoadAssetByLabel => loadResourceLocationLabelQueue != null && loadResourceLocationLabelQueue.Count > 0;
        public int TotalLoadAssetCount => loadAssetResultDict.Count;
        public LoadingProgress CurrentLoadProgress { get; private set; }
        private ILoadAssetSetting LoadAssetSetting { get; }

        public LoadAssetProcess(ILoadAssetSetting loadAssetSetting, Debugger debugger)
        {
            this.debugger = debugger;
            LoadAssetSetting = loadAssetSetting;
            InitLoadAssetNames();
            InitLoadResourceLocations();
        }

        private void InitLoadResourceLocations()
        {
            loadResourceLocationLabelQueue = new Queue<string>();

            string[] loadAssetLabels = LoadAssetSetting.GetLoadAssetLabels;
            if (loadAssetLabels == null || loadAssetLabels.Length == 0)
                return;

            foreach (string label in loadAssetLabels)
            {
                loadResourceLocationLabelQueue.Enqueue(label);
            }
        }

        private void InitLoadAssetNames()
        {
            foreach (string assetName in LoadAssetSetting.GetLoadAssetNames)
            {
                if (string.IsNullOrEmpty(assetName))
                    continue;

                loadAssetResultDict[assetName] = new LoadingAssetResource(assetName);
            }
        }

        public string GetNextLoadLabel()
        {
            return loadResourceLocationLabelQueue.Count == 0 ?
                string.Empty :
                loadResourceLocationLabelQueue.Dequeue();
        }

        public LoadAssetKey GetNextLoadAssetKey()
        {
            LoadAssetKey nextLoadKey = null;
            currentLoadAsset = null;
            if (loadAssetResourceQueue != null && loadAssetResourceQueue.Count > 0)
            {
                LoadingAssetResource nextLoadAsset = loadAssetResourceQueue.Dequeue();
                nextLoadKey = nextLoadAsset.LoadAssetKey;
                currentLoadAsset = nextLoadAsset;
            }

            return nextLoadKey;
        }

        public Dictionary<string, object> ParseLoadedAssetResult()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            if (loadAssetResultDict == null || loadAssetResultDict.Count == 0)
                return result;

            foreach (LoadingAssetResource assetResource in loadAssetResultDict.Values)
            {
                if (assetResource.IsLoadedSuccess)
                    result[assetResource.LoadAssetKey.Key] = assetResource.LoadedObjResult;
            }

            return result;
        }

        public void SetLoadedResourceLocation(IResourceLocation resourceLocation)
        {
            if (resourceLocation == null || string.IsNullOrEmpty(resourceLocation.PrimaryKey))
                return;

            if (resourceLocationDict.ContainsKey(resourceLocation.PrimaryKey))
            {
                if (IsNeedReplace(resourceLocation.ResourceType) == false)
                    return;

                debugger.ShowLog($"ResourceLocation is already exist and replace it, PrimaryKey: {resourceLocation.PrimaryKey}");
                resourceLocationDict[resourceLocation.PrimaryKey] = resourceLocation;
            }
            else
                resourceLocationDict[resourceLocation.PrimaryKey] = resourceLocation;

            loadAssetResultDict[resourceLocation.PrimaryKey] = new LoadingAssetResource(resourceLocation);
        }

        public void SetLoadedAsset<T>(AsyncOperationHandle<T> loadedObj, out string assetName)
        {
            assetName = string.Empty;

            if (loadedObj.Status == AsyncOperationStatus.Succeeded)
            {
                // if (currentLoadAsset.CompareLoadedObjectName(loadedObj.Result.name))
                {
                    loadAssetResultDict[currentLoadAsset.LoadAssetKey.Key].SetLoadedAsset(loadedObj.Result);
                    CurrentLoadProgress.AddLoadedAsset(loadedObj.Result);
                    assetName = currentLoadAsset.LoadAssetKey.Key;
                }
                // else
                // throw new Exception(
                // $"Loaded object name is not matched with current load asset name, LoadedObjName: {loadedObj.Result.name}, CurrentLoadAssetName: {currentLoadAsset.LoadAssetKey.Key}");
            }
            else
                CurrentLoadProgress.AddLoadedAsset(string.Empty);
        }

        public void SetBypassLoadedAsset(string assetName)
        {
            loadAssetResultDict.Remove(currentLoadAsset.LoadAssetKey.Key);
            CurrentLoadProgress.AddLoadedAsset(assetName);
        }

        public void StartLoadAsset()
        {
            CurrentLoadProgress = new LoadingProgress(TotalLoadAssetCount);
            currentLoadAsset = null;
            loadAssetResourceQueue = GetLoadAssetResourceQueue();

            string logs = string.Join("\n", loadAssetResultDict.Keys);
            debugger.ShowLog($"Load Asset List:\n{logs}", true);
        }

        public void PrintLoadAssetResultLog()
        {
            List<string> logs = new List<string>();
            foreach (LoadingAssetResource assetResource in loadAssetResultDict.Values)
            {
                logs.Add(
                    $"AssetName: {assetResource.LoadAssetKey.Key}, LoadType: {assetResource.LoadAssetKey.KeyType}, IsLoadedSuccess: {assetResource.IsLoadedSuccess}");
            }

            debugger.ShowLog($"Load Asset Result logs:\n{string.Join("\n", logs)}");
        }

        private bool IsNeedReplace(Type resourceType)
        {
            return resourceType.IsSubclassOf(typeof(GameObject)) ||
                   resourceType.IsSubclassOf(typeof(ScriptableObject)) ||
                   resourceType.IsSubclassOf(typeof(TextAsset));
        }

        private Queue<LoadingAssetResource> GetLoadAssetResourceQueue()
        {
            return new Queue<LoadingAssetResource>(loadAssetResultDict.Values);
        }
    }
}
#endif
