using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SNShien.Common.MonoBehaviorTools
{
    public class RandomPositionInRect : MonoBehaviour
    {
        [SerializeField] private bool isShowEditorDrawer;
        [SerializeField] private Vector2 rectMin = new Vector2(-1, -1);
        [SerializeField] private Vector2 rectMax = new Vector2(1, 1);
        [SerializeField] private Color rectColor = Color.green;

        public bool IsShowEditorDrawer => isShowEditorDrawer;
        public Vector3 RectMin => rectMin;
        public Vector3 RectMax => rectMax;
        public Color RectColor => rectColor;

        public Vector3 GetRandomPosition()
        {
            float x = Random.Range(rectMin.x, rectMax.x);
            float y = Random.Range(rectMin.y, rectMax.y);
            return new Vector3(x, y, 0);
        }

        public void SetRect(Vector2 newRectMin, Vector2 newRectMax)
        {
            rectMin = newRectMin;
            rectMax = newRectMax;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = rectColor;
            Vector3 size = new Vector3(rectMax.x - rectMin.x, rectMax.y - rectMin.y, 0);
            Vector3 center = new Vector3((rectMin.x + rectMax.x) / 2, (rectMin.y + rectMax.y) / 2, 0);
            Gizmos.DrawWireCube(center, size);
        }
# endif
    }
}