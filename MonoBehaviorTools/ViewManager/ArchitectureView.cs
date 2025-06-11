#if CUSTOM_USING_ODIN
using Sirenix.OdinInspector;
using SNShien.Common.TesterTools;
using UnityEngine;

namespace SNShien.Common.MonoBehaviorTools
{
    public abstract class ArchitectureView : SerializedMonoBehaviour
    {
        [SerializeField] private bool isAutoResponsiveBySafeArea;
        [ShowIf("isAutoResponsiveBySafeArea")] [SerializeField] private RectTransform safeAreaRectRoot;

        private Canvas canvas;

        private readonly Debugger debugger = new Debugger("ArchitectureView");

        public int CanvasSortOrder
        {
            get
            {
                return Canvas == null ?
                    0 :
                    Canvas.sortingOrder;
            }
            set
            {
                if (Canvas != null)
                    Canvas.sortingOrder = value;
            }
        }

        private Canvas Canvas
        {
            get
            {
                if (canvas == null)
                    canvas = gameObject.GetComponentInChildren<Canvas>();

                return canvas;
            }
        }

        public void InitCanvasRenderModeSetting()
        {
            if (Canvas == null)
                return;

            Canvas.renderMode = RenderMode.ScreenSpaceCamera;
            Canvas.worldCamera = Camera.main;
        }

        public void InitSafeAreaSetting()
        {
            InitSafeAreaSetting(Screen.width, Screen.height);
        }

        public void InitSafeAreaSetting(int screenWidth, int screenHeight)
        {
            debugger.ShowLog($"Screen.width: {Screen.width}, Screen.height: {Screen.height}", true);
            
            if (isAutoResponsiveBySafeArea == false)
                return;

            Rect safeArea = Screen.safeArea;

            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= screenWidth;
            anchorMin.y /= screenHeight;
            anchorMax.x /= screenWidth;
            anchorMax.y /= screenHeight;

            safeAreaRectRoot.anchorMin = anchorMin;
            safeAreaRectRoot.anchorMax = anchorMax;
        }

        public abstract void UpdateView();
        public abstract void OpenView(params object[] parameters);
        public abstract void ReOpenView(params object[] parameters);
        public abstract void CloseView();
    }
}
#endif