using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SNShien.Common.TesterTools;
using UnityEditor;
using UnityEngine;

namespace SNShien.Common.MonoBehaviorTools
{
    public class ScenePrefabPreviewer : SerializedMonoBehaviour
    {
        [ReadOnly] [ShowInInspector] private ScenePrefabPreviewState previewState = ScenePrefabPreviewState.None;

        [SerializeField] private List<GameObject> prefabList;
        [SerializeField] private bool isAutoResponsiveBySafeArea;

        [SerializeField] [ShowIf("isAutoResponsiveBySafeArea")] private DeviceScreenSizeReferenceScriptableObject deviceScreenSizeReferenceSetting;

        [SerializeField] [ShowIf("$CanShowDeviceTypeDropdown")] [ValueDropdown("$GetDeviceTypeList")]
        private string deviceType = string.Empty;

        [SerializeField] private bool isAutoSetupSortOrderByViewSetting;

        [SerializeField] [ShowIf("isAutoSetupSortOrderByViewSetting")]
        private ViewPrefabScriptableObject viewSetting;

        private Camera camera;

        private readonly List<GameObject> currentPrefabsInScene = new List<GameObject>();
        private readonly Debugger debugger = new Debugger("ScenePrefabPreviewer");

        private List<GameObject> GetPrefabList => prefabList;

        [Button("產生Prefab預覽")]
        public void EditorButton1_CreatePrefabPreview()
        {
            SetupCamera();
            CreatePrefabs();
            SetupAllCanvas();
            SetupSafeAreaSetting();
        }

        [Button("清除Prefab預覽")]
        public void EditorButton2_ClearPrefabPreview()
        {
            foreach (GameObject go in currentPrefabsInScene)
            {
                DestroyImmediate(go);
            }

            currentPrefabsInScene.Clear();
            previewState = ScenePrefabPreviewState.None;
        }

        private bool CanShowDeviceTypeDropdown()
        {
            return isAutoResponsiveBySafeArea && deviceScreenSizeReferenceSetting != null;
        }

        private IEnumerable<string> GetDeviceTypeList()
        {
            if (deviceScreenSizeReferenceSetting == null || deviceScreenSizeReferenceSetting.IsSettingEmpty)
                return new List<string>();

            return deviceScreenSizeReferenceSetting.GetDeviceTypeStringList;
        }

        private void SetupSafeAreaSetting()
        {
            if (isAutoResponsiveBySafeArea == false)
                return;

            foreach (GameObject go in currentPrefabsInScene)
            {
                ArchitectureView view = go.GetComponent<ArchitectureView>();
                if (view != null)
                {
                    Vector2Int screenSize = deviceScreenSizeReferenceSetting.GetScreenSize(deviceType);
                    view.InitSafeAreaSetting(screenSize.x, screenSize.y);
                }
            }
        }

        private void SetupAllCanvas()
        {
            foreach (Canvas canvas in FindObjectsOfType<Canvas>())
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = camera;
            }

            if (isAutoSetupSortOrderByViewSetting && viewSetting != null)
            {
                Dictionary<Type, int> viewSortOrderDict = viewSetting.GetViewSortOrderDict();
                foreach (ArchitectureView view in FindObjectsOfType<ArchitectureView>())
                {
                    view.CanvasSortOrder = viewSortOrderDict[view.GetType()];
                }
            }
        }

        private void SetupCamera()
        {
            camera = default;
            foreach (Camera c in FindObjectsOfType<Camera>())
            {
                if (c.CompareTag("MainCamera"))
                {
                    camera = c;
                    break;
                }
            }
        }

        private void CreatePrefabs()
        {
            if (previewState == ScenePrefabPreviewState.Previewing)
                return;

            List<GameObject> prefabList = FindObjectOfType<ScenePrefabPreviewer>().GetPrefabList;
            if (prefabList == null || prefabList.Count == 0)
                return;

            foreach (GameObject prefab in prefabList)
            {
                GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                currentPrefabsInScene.Add(instance);
            }

            previewState = ScenePrefabPreviewState.Previewing;
        }
    }
}