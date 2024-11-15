using UnityEngine;

namespace SNShien.Common.MonoBehaviorTools
{
    public abstract class ArchitectureView : MonoBehaviour
    {
        private Canvas canvas;

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
            Canvas.renderMode = RenderMode.ScreenSpaceCamera;
            Canvas.worldCamera = Camera.main;
        }

        public void SetCanvasSortOrder(int sortOrder)
        {
            Canvas.sortingOrder = sortOrder;
        }

        public abstract void UpdateView();
        public abstract void OpenView(params object[] parameters);
        public abstract void ReOpenView(params object[] parameters);
        public abstract void CloseView();
    }
}