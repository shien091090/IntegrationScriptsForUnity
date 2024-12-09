using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using SNShien.Common.AdapterTools;
using SNShien.Common.ProcessTools;
using SNShien.Common.TesterTools;
using UnityEngine;
using Zenject;

namespace SNShien.Common.MonoBehaviorTools
{
    public class ViewManager : SerializedMonoBehaviour, IViewManager
    {
        private const string DEBUGGER_KEY = "ViewManager";

        [Inject] private IViewPrefabSetting viewPrefabSetting;
        [Inject] private IEventRegister eventRegister;
        [InjectOptional] private IGameObjectSpawner gameObjectSpawner;

        [SerializeField] private Transform viewHolder;

        private readonly Dictionary<Type, InSceneViewInfo> viewStateDict = new Dictionary<Type, InSceneViewInfo>();
        private readonly Debugger debugger = new Debugger(DEBUGGER_KEY);

        private Dictionary<Type, ArchitectureView> viewPrefabDict = new Dictionary<Type, ArchitectureView>();
        private Dictionary<Type, int> viewSortOrderDict = new Dictionary<Type, int>();
        private bool isInit;

        public void OpenView<T>(params object[] parameters) where T : ArchitectureView
        {
            ViewState currentViewState = GetCurrentViewState<T>();
            switch (currentViewState)
            {
                case ViewState.Opened:
                    GetViewInstance<T>().UpdateView();
                    break;

                case ViewState.Closed:
                    GetViewInstance<T>().ReOpenView(parameters);
                    break;

                case ViewState.NotExist:
                    {
                        if (viewPrefabDict.TryGetValue(typeof(T), out ArchitectureView prefab) == false)
                            return;

                        ArchitectureView view = CreateNewView<T>(prefab.gameObject);
                        view.OpenView(parameters);
                        SetViewState<T>(ViewState.Opened);
                        break;
                    }
            }
        }

        public void ExecuteModelInit()
        {
            Init();
        }

        public void Release()
        {
            SetEventRegister(false);
        }

        public void ClearAllView()
        {
            foreach (InSceneViewInfo viewInfo in viewStateDict.Values)
            {
                debugger.ShowLog($"CloseAllView, view: {viewInfo.View.GetType().Name}");
                viewInfo.View.CloseView();
            }

            viewStateDict.Clear();

            List<GameObject> destroyList = new List<GameObject>();
            for (int i = 0; i < viewHolder.childCount; i++)
            {
                GameObject go = viewHolder.GetChild(i).gameObject;
                destroyList.Add(go);
            }

            foreach (GameObject go in destroyList)
            {
                DestroyImmediate(go);
            }
        }

        public int GetViewSortOrder<T>() where T : ArchitectureView
        {
            return GetViewSortOrder(typeof(T));
        }

        public int GetViewSortOrder(Type type)
        {
            viewSortOrderDict.TryGetValue(type, out int sortOrder);
            return sortOrder;
        }

        private void Init()
        {
            if (isInit)
                return;

            InitViewPrefabDict();
            SetEventRegister(true);
            isInit = true;
        }

        private void InitViewPrefabDict()
        {
            viewPrefabDict = new Dictionary<Type, ArchitectureView>();

            List<ArchitectureView> prefabList = new List<ArchitectureView>();
            prefabList.AddRange(viewPrefabSetting.GetPrefabList);

            foreach (ArchitectureView viewPrefab in prefabList)
            {
                viewPrefabDict[viewPrefab.GetType()] = viewPrefab;
            }

            prefabList.Reverse();
            int sortOrder = 0;
            foreach (ArchitectureView viewPrefab in prefabList)
            {
                viewSortOrderDict[viewPrefab.GetType()] = sortOrder;
                sortOrder += 10;
            }

            PrintInitViewPrefabDictLog();
        }

        private ArchitectureView GetViewInstance<T>() where T : ArchitectureView
        {
            if (viewStateDict.ContainsKey(typeof(T)))
                return viewStateDict[typeof(T)].View;
            else
            {
                viewStateDict[typeof(T)] = new InSceneViewInfo();
                return null;
            }
        }

        private ViewState GetCurrentViewState<T>() where T : ArchitectureView
        {
            if (viewStateDict.ContainsKey(typeof(T)))
                return viewStateDict[typeof(T)].CurrentViewState;
            else
            {
                viewStateDict[typeof(T)] = new InSceneViewInfo();
                return ViewState.NotExist;
            }
        }

        private void SetViewState<T>(ViewState state) where T : ArchitectureView
        {
            if (viewStateDict.TryGetValue(typeof(T), out InSceneViewInfo viewInfo))
                viewInfo.SetState(state);
        }

        private void SetEventRegister(bool isListen)
        {
            eventRegister.Unregister<SwitchSceneEvent>(OnSwitchSceneEvent);

            if (isListen)
            {
                eventRegister.Register<SwitchSceneEvent>(OnSwitchSceneEvent);
            }
        }

        private void Awake()
        {
            Init();
        }

        private void PrintInitViewPrefabDictLog()
        {
            List<string> viewPrefabNameList = viewPrefabDict.Values.Select(x => x.name).ToList();
            for (int i = 0; i < viewPrefabNameList.Count; i++)
            {
                string viewPrefabName = viewPrefabNameList[i];
                viewPrefabNameList[i] = $"{i + 1}. {viewPrefabName}";
            }

            string log = viewPrefabNameList.Count == 0 ?
                "{Empty}" :
                string.Join("\n", viewPrefabNameList);

            debugger.ShowLog($"InitViewPrefabDict, view prefab list count: {viewPrefabNameList.Count}, list:\n{log}");
        }

        private ArchitectureView CreateNewView<T>(GameObject prefab) where T : ArchitectureView
        {
            GameObject newGo = gameObjectSpawner == null ?
                Instantiate(prefab, viewHolder) :
                gameObjectSpawner.Spawn(prefab, viewHolder);

            debugger.ShowLog($"CreateNewView, prefab: {prefab.name}, isUseGameObjectSpawner: {gameObjectSpawner != null}");

            ArchitectureView view = newGo.GetComponent<ArchitectureView>();
            viewStateDict[typeof(T)] = new InSceneViewInfo(view);

            view.InitCanvasRenderModeSetting();
            view.InitSafeAreaSetting();
            view.CanvasSortOrder = GetViewSortOrder<T>();

            return view;
        }

        private void OnSwitchSceneEvent(SwitchSceneEvent eventInfo)
        {
            ClearAllView();
        }
    }
}