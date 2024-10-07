using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SNShien.Common.ProcessTools;
using UnityEngine;

namespace SNShien.Common.MonoBehaviorTools
{
    public class ViewManager : SerializedMonoBehaviour, IViewManager
    {
        [SerializeField] private ZenjectGameObjectSpawner zenjectGameObjectSpawner;
        [SerializeField] private Transform viewHolder;

        private Dictionary<Type, GameObject> viewPrefabDict = new Dictionary<Type, GameObject>();
        private Dictionary<Type, InSceneViewInfo> viewStateDict;

        private bool IsUseZenjectGameObjectSpawner => zenjectGameObjectSpawner != null;

        public void OpenView<T>(params object[] parameters) where T : IArchitectureView
        {
            ViewState currentViewState = GetCurrentViewState<T>();
            if(currentViewState == ViewState.Closed)

            if (viewPrefabDict.TryGetValue(typeof(T), out GameObject prefab) == false)
                return;

            GameObject newGo = null;
            newGo = IsUseZenjectGameObjectSpawner ?
                zenjectGameObjectSpawner.Spawn(prefab, viewHolder) :
                Instantiate(prefab, viewHolder);

            IArchitectureView view = newGo.GetComponent<IArchitectureView>();
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
    }
}