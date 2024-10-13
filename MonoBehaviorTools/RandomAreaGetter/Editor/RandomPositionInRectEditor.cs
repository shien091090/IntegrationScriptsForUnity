using UnityEditor;
using UnityEngine;

namespace SNShien.Common.MonoBehaviorTools
{
    [CustomEditor(typeof(RandomPositionInRect))]
    public class RandomPositionInRectEditor : Editor
    {
        private void OnSceneGUI()
        {
            RandomPositionInRect component = (RandomPositionInRect)target;
            if (component.IsShowEditorDrawer == false)
                return;

            EditorGUI.BeginChangeCheck();
            Vector2 newRectMin = Handles.PositionHandle(component.RectMin, Quaternion.identity);
            Vector2 newRectMax = Handles.PositionHandle(component.RectMax, Quaternion.identity);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(component, "Adjust Rectangle");

                component.SetRect(newRectMin, newRectMax);

                EditorUtility.SetDirty(component);
            }

            Vector3[] rectPoints = new Vector3[4]
            {
                new Vector3(component.RectMin.x, component.RectMin.y, 0),
                new Vector3(component.RectMax.x, component.RectMin.y, 0),
                new Vector3(component.RectMax.x, component.RectMax.y, 0),
                new Vector3(component.RectMin.x, component.RectMax.y, 0)
            };

            Handles.DrawSolidRectangleWithOutline(rectPoints, component.RectColor * 0.5f, Color.green);
        }
    }
}