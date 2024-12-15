using UnityEngine;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SNShien.Common.MonoBehaviorTools
{
    [RequireComponent(typeof(RectTransform))]
    public class RandomPositionInRect : MonoBehaviour
    {
        [SerializeField] private bool isShowEditorDrawer;
        [SerializeField] private Color rectColor = Color.green;
        [SerializeField] private float outlineWidth = 5;

        private RectTransform rectTransform;

        private RectTransform RectTransform
        {
            get
            {
                if (rectTransform == null)
                    rectTransform = gameObject.GetComponent<RectTransform>();

                return rectTransform;
            }
        }

        public Vector3 GetRandomPosition()
        {
            Vector2 rectMin = RectTransform.rect.min;
            Vector2 rectMax = RectTransform.rect.max;
            float x = Random.Range(rectMin.x, rectMax.x);
            float y = Random.Range(rectMin.y, rectMax.y);
            return new Vector3(x, y, 0);
        }

        public Vector3 GetRandomLocalPosition()
        {
            float halfSizeDeltaX = RectTransform.sizeDelta.x / 2;
            float halfSizeDeltaY = RectTransform.sizeDelta.y / 2;
            float x = Random.Range(-halfSizeDeltaX, halfSizeDeltaX);
            float y = Random.Range(-halfSizeDeltaY, halfSizeDeltaY);
            return new Vector3(x, y, 0);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (isShowEditorDrawer == false)
                return;

            Vector3[] corners = new Vector3[4];
            RectTransform.GetWorldCorners(corners);
            Vector3[] rectCorners = new Vector3[5];
            for (int i = 0; i < 4; i++)
            {
                rectCorners[i] = corners[i];
            }

            rectCorners[4] = corners[0];
            for (int i = 0; i < 4; i++)
            {
                Handles.color = rectColor;
                Handles.DrawAAPolyLine(outlineWidth, rectCorners[i], rectCorners[i + 1]);
            }
        }
# endif
    }
}