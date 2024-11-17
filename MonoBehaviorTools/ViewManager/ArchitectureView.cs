using UnityEngine;

namespace SNShien.Common.MonoBehaviorTools
{
    public abstract class ArchitectureView : MonoBehaviour
    {
        private Canvas canvas;

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

        public abstract void UpdateView();
        public abstract void OpenView(params object[] parameters);
        public abstract void ReOpenView(params object[] parameters);
        public abstract void CloseView();
    }
}