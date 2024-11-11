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

        private Dictionary<Type, GameObject> viewPrefabDict = new Dictionary<Type, GameObject>();
        private bool isInit;

        public void OpenView<T>(params object[] parameters) where T : IArchitectureView
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
                        if (viewPrefabDict.TryGetValue(typeof(T), out GameObject prefab) == false)
                            return;

                        CreateNewView<T>(prefab).OpenView(parameters);
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

        private void Awake()
        {
            Init();
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
            viewPrefabDict = new Dictionary<Type, GameObject>();
            foreach (GameObject prefab in viewPrefabSetting.GetPrefabList)
            {
                IArchitectureView view = prefab.GetComponent<IArchitectureView>();
                if (view == null)
                    debugger.ShowLog($"Prefab {prefab.name} doesn't have IArchitectureView component.");
                else
                    viewPrefabDict[view.GetType()] = prefab;
            }

            PrintInitViewPrefabDictLog();
        }

        private IArchitectureView GetViewInstance<T>() where T : IArchitectureView
        {
            if (viewStateDict.ContainsKey(typeof(T)))
                return viewStateDict[typeof(T)].View;
            else
            {
                viewStateDict[typeof(T)] = new InSceneViewInfo();
                return null;
            }
        }

        private ViewState GetCurrentViewState<T>() where T : IArchitectureView
        {
            if (viewStateDict.ContainsKey(typeof(T)))
                return viewStateDict[typeof(T)].CurrentViewState;
            else
            {
                viewStateDict[typeof(T)] = new InSceneViewInfo();
                return ViewState.NotExist;
            }
        }

        private void SetViewState<T>(ViewState state) where T : IArchitectureView
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

        private IArchitectureView CreateNewView<T>(GameObject prefab) where T : IArchitectureView
        {
            GameObject newGo = gameObjectSpawner == null ?
                Instantiate(prefab, viewHolder) :
                gameObjectSpawner.Spawn(prefab, viewHolder);

            debugger.ShowLog($"CreateNewView, prefab: {prefab.name}, isUseGameObjectSpawner: {gameObjectSpawner != null}");

            IArchitectureView view = newGo.GetComponent<IArchitectureView>();
            viewStateDict[typeof(T)] = new InSceneViewInfo(view);

            return view;
        }

        private void OnSwitchSceneEvent(SwitchSceneEvent eventInfo)
        {
            ClearAllView();
        }
    }
}