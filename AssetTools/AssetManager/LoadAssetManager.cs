#if CUSTOM_USING_ZENJECT && CUSTOM_USING_ADDRESSABLE
using System;
using System.Collections.Generic;
using System.Linq;
using SNShien.Common.TesterTools;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using Zenject;
using Object = UnityEngine.Object;

namespace SNShien.Common.AssetTools
{
    public class LoadAssetManager : IAssetManager
    {
        private const string DEBUGGER_KEY = "LoadAssetManager";

        [Inject] private ILoadAssetSetting loadAssetSetting;

        private readonly Debugger debugger;

        private Dictionary<string, object> assetDict = new Dictionary<string, object>();
        private AssetLoadingState currentState;
        private LoadAssetProcess loadAssetProcess;

        public LoadAssetManager()
        {
            debugger = new Debugger(DEBUGGER_KEY);
            SwitchCurrentState(AssetLoadingState.None);
        }

        public event Action<AssetLoadingState> OnUpdateLoadingState;
        public event Action<LoadingProgress> OnUpdateLoadingProgress;
        public event Action OnAllAssetLoadCompleted;

        public T GetAsset<T>(string assetName)
        {
            if (!assetDict.ContainsKey(assetName))
                return default;

            object obj = assetDict[assetName];
            if (obj is T result)
                return result;

            if (!(obj is GameObject go))
                return default;

            T component = go.GetComponent<T>();
            return component != null ?
                component :
                default;
        }

        public void StartPrecedingProcedures()
        {
            SwitchCurrentState(AssetLoadingState.StartLoad);
            loadAssetProcess = new LoadAssetProcess(loadAssetSetting, debugger);

            if (loadAssetProcess.IsNeedLoadAssetByLabel)
            {
                SwitchCurrentState(AssetLoadingState.LoadResourceLocation);
                LoadResourceLocationQueue();
            }
            else
                StartLoadAssetQueue();
        }

        private void SwitchCurrentState(AssetLoadingState state)
        {
            if (state == currentState)
                return;

            debugger.ShowLog($"{currentState} -> {state}", true);
            currentState = state;

            if (state == AssetLoadingState.LoadAssets)
                loadAssetProcess.StartLoadAsset();

            OnUpdateLoadingState?.Invoke(currentState);
        }

        private void StartLoadAssetQueue()
        {
            SwitchCurrentState(AssetLoadingState.LoadAssets);
            debugger.ShowLog($"totalAssetCount: {loadAssetProcess.TotalLoadAssetCount}", true);

            OnUpdateLoadingProgress?.Invoke(loadAssetProcess.CurrentLoadProgress);
            LoadAssetQueue();
        }

        private void LoadResourceLocationQueue()
        {
            string nextLoadLabel = loadAssetProcess.GetNextLoadLabel();

            if (string.IsNullOrEmpty(nextLoadLabel))
                StartLoadAssetQueue();
            else
                Addressables.LoadResourceLocationsAsync(nextLoadLabel).Completed += OnLoadResourceLocationCompleted;
        }

        private void LoadAssetQueue()
        {
            LoadAssetKey nextLoadAssetKey = loadAssetProcess.GetNextLoadAssetKey();
            if (nextLoadAssetKey == null)
            {
                AllAssetLoadCompleted();
                return;
            }

            switch (nextLoadAssetKey.KeyType)
            {
                case LoadAssetKeyType.ResourceLocation:
                    Addressables.LoadAssetAsync<object>(nextLoadAssetKey.ResourceLocation).Completed += OnLoadAssetCompleted;
                    break;

                case LoadAssetKeyType.ResourceLocation_Scene:
                    BypassLoadAsset(nextLoadAssetKey);
                    break;

                default:
                    Addressables.LoadAssetAsync<object>(nextLoadAssetKey.Key).Completed += OnLoadAssetCompleted;
                    break;
            }
        }

        private void BypassLoadAsset(LoadAssetKey assetKey)
        {
            loadAssetProcess.SetBypassLoadedAsset(assetKey.Key);

            debugger.ShowLog(
                $"Key: {assetKey.Key}, KeyType: {assetKey.KeyType}, Progress: {loadAssetProcess.CurrentLoadProgress.LoadedCount}/{loadAssetProcess.CurrentLoadProgress.TotalAssetCount}",
                true);

            OnUpdateLoadingProgress?.Invoke(loadAssetProcess.CurrentLoadProgress);
            LoadAssetQueue();
        }

        private void AllAssetLoadCompleted()
        {
            SwitchCurrentState(AssetLoadingState.LoadCompleted);
            loadAssetProcess.PrintLoadAssetResultLog();
            assetDict = loadAssetProcess.ParseLoadedAssetResult();
            OnAllAssetLoadCompleted?.Invoke();
            loadAssetProcess = null;
        }

        private void OnLoadResourceLocationCompleted(AsyncOperationHandle<IList<IResourceLocation>> loadedObj)
        {
            foreach (IResourceLocation resourceLocation in loadedObj.Result)
            {
                loadAssetProcess.SetLoadedResourceLocation(resourceLocation);
            }

            List<string> logs = loadedObj.Result
                .Select(resourceLocation => $"PrimaryKey: {resourceLocation.PrimaryKey}, ResourceType: {resourceLocation.ResourceType}")
                .ToList();

            debugger.ShowLog($"ResourceLocation logs:\n{string.Join("\n", logs)}", true);

            LoadResourceLocationQueue();
        }

        private void OnLoadAssetCompleted<T>(AsyncOperationHandle<T> loadedObj)
        {
            loadAssetProcess.SetLoadedAsset(loadedObj, out string assetName);

            debugger.ShowLog(
                $"Name: {assetName}, Status: {loadedObj.Status}, Progress: {loadAssetProcess.CurrentLoadProgress.LoadedCount}/{loadAssetProcess.CurrentLoadProgress.TotalAssetCount}",
                true);

            OnUpdateLoadingProgress?.Invoke(loadAssetProcess.CurrentLoadProgress);
            LoadAssetQueue();
        }
    }
}
#endif