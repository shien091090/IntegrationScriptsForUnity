using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
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

        [SerializeField] private ZenjectGameObjectSpawner zenjectGameObjectSpawner;
        [SerializeField] private Transform viewHolder;

        private Dictionary<Type, GameObject> viewPrefabDict = new Dictionary<Type, GameObject>();
        private Dictionary<Type, InSceneViewInfo> viewStateDict;
        private Debugger debugger = new Debugger(DEBUGGER_KEY);
        private bool isInit;

        private bool IsUseZenjectGameObjectSpawner => zenjectGameObjectSpawner != null;

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

        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            debugger.ShowLog("Init");
            if (isInit)
                return;

            InitViewPrefabDict();
        }

        private void InitViewPrefabDict()
        {
            viewPrefabDict = new Dictionary<Type, GameObject>();
            foreach (GameObject prefab in viewPrefabSetting.GetPrefabList)
            {
                viewPrefabDict[prefab.GetComponent<IArchitectureView>().GetType()] = prefab;
            }

            //print log
            foreach (KeyValuePair<Type, GameObject> item in viewPrefabDict)
            {
                Debug.Log($"{item.Key.Name} : {item.Value.name}");
            }
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

        private IArchitectureView CreateNewView<T>(GameObject prefab) where T : IArchitectureView
        {
            GameObject newGo = null;
            newGo = IsUseZenjectGameObjectSpawner ?
                zenjectGameObjectSpawner.Spawn(prefab, viewHolder) :
                Instantiate(prefab, viewHolder);

            IArchitectureView view = newGo.GetComponent<IArchitectureView>();
            viewStateDict[typeof(T)] = new InSceneViewInfo(view);

            return view;
        }
    }
}