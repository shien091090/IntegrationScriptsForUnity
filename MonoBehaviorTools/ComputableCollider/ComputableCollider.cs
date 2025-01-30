using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SNShien.Common.MonoBehaviorTools
{
    public class ComputableCollider : MonoBehaviour
    {
        [SerializeField] private ComputableColliderShapeType shapeType;

        [Header("Debug Hint")] [SerializeField] private bool isShowHighlightHintForTest;
        [SerializeField] private Image img_highlightForTest;

        private RectTransform rectTransform;
        private ComputableCollider currentTrackingTarget;

        public event Action<bool, ComputableCollider> OnChangeTriggeredState;

        public Vector3 Position => RectTransform.position;
        private bool CurrentTriggeredState { get; set; }
        private ComputableColliderShapeType ShapeType => shapeType;
        private RectTransform RectTransform => rectTransform;

        private void Update()
        {
            if (currentTrackingTarget == null)
                return;

            bool isInRange = IsInRange(currentTrackingTarget.ShapeType, currentTrackingTarget.RectTransform);
            if (isInRange && CurrentTriggeredState == false)
                SetTriggeredState(true, currentTrackingTarget);
            else if (isInRange == false && CurrentTriggeredState)
                SetTriggeredState(false, currentTrackingTarget);
        }

        public void RemoveTrackingTarget()
        {
            SetTriggeredState(false, currentTrackingTarget);
            currentTrackingTarget = null;
        }

        public void StartTrackingTarget(ComputableCollider target)
        {
            currentTrackingTarget = target;
            CurrentTriggeredState = false;
        }

        private bool IsInRange(ComputableColliderShapeType targetShapeType, RectTransform targetRectTransform)
        {
            Vector3[] targetCorners = new Vector3[4];
            targetRectTransform.GetWorldCorners(targetCorners);

            Vector3[] selfCorners = new Vector3[4];
            RectTransform.GetWorldCorners(selfCorners);

            Vector3 targetCenter = targetRectTransform.position;
            Vector3 selfCenter = RectTransform.position;

            float targetCircleRadius = targetRectTransform.sizeDelta.y / 2 * targetRectTransform.lossyScale.y;
            float selfCircleRadius = RectTransform.sizeDelta.y / 2 * RectTransform.lossyScale.y;

            if (targetShapeType == ComputableColliderShapeType.Circle)
            {
                switch (ShapeType)
                {
                    case ComputableColliderShapeType.Rectangle:
                        return IsCircleRectangleOverlap(targetCenter, targetCircleRadius, selfCorners);

                    case ComputableColliderShapeType.Circle:
                        return Vector3.Distance(targetCenter, selfCenter) <= targetCircleRadius + selfCircleRadius;
                }
            }
            else if (targetShapeType == ComputableColliderShapeType.Rectangle)
            {
                switch (ShapeType)
                {
                    case ComputableColliderShapeType.Rectangle:
                        return IsRectangleOverlap(selfCorners, targetCorners);

                    case ComputableColliderShapeType.Circle:
                        return IsCircleRectangleOverlap(selfCenter, selfCircleRadius, targetCorners);
                }
            }

            return false;
        }

        private bool IsRectangleOverlap(Vector3[] rect1Corners, Vector3[] rect2Corners)
        {
            // 使用分離軸定理 (SAT)
            return CheckOverlapUsingSAT(rect1Corners, rect2Corners);
        }

        private bool IsCircleRectangleOverlap(Vector3 circleCenter, float circleRadius, Vector3[] rectCorners)
        {
            Vector3 closestPoint = ClosestPointOnRectangle(circleCenter, rectCorners);
            return Vector3.Distance(circleCenter, closestPoint) <= circleRadius;
        }

        private bool IsProjectionOverlapping(Vector3[] rect1Corners, Vector3[] rect2Corners, Vector3 axis)
        {
            GetProjectionRange(rect1Corners, axis, out float rect1Min, out float rect1Max);
            GetProjectionRange(rect2Corners, axis, out float rect2Min, out float rect2Max);

            return !(rect1Max < rect2Min || rect2Max < rect1Min);
        }

        private bool IsShowHighlightHintForTest()
        {
#if !UNITY_EDITOR
     return false;
#endif
            return isShowHighlightHintForTest && img_highlightForTest != null;
        }

        private Vector3[] GetAxesFromRectangle(Vector3[] corners)
        {
            Vector3[] axes = new Vector3[2];
            axes[0] = (corners[1] - corners[0]).normalized;
            axes[1] = (corners[2] - corners[1]).normalized;
            return axes;
        }

        private void GetProjectionRange(Vector3[] corners, Vector3 axis, out float min, out float max)
        {
            min = max = Vector3.Dot(corners[0], axis);

            for (int i = 1; i < corners.Length; i++)
            {
                float projection = Vector3.Dot(corners[i], axis);
                if (projection < min) min = projection;
                if (projection > max) max = projection;
            }
        }

        private void SetTriggeredState(bool isTriggered, ComputableCollider target)
        {
            if (CurrentTriggeredState == isTriggered)
                return;

            CurrentTriggeredState = isTriggered;
            OnChangeTriggeredState?.Invoke(CurrentTriggeredState, target);

            if (IsShowHighlightHintForTest())
                ShowHighlightHint(CurrentTriggeredState);
        }

        private bool CheckOverlapUsingSAT(Vector3[] rect1Corners, Vector3[] rect2Corners)
        {
            Vector3[] axes = GetAxesFromRectangle(rect1Corners).Concat(GetAxesFromRectangle(rect2Corners)).ToArray();
            return axes.All(axis => IsProjectionOverlapping(rect1Corners, rect2Corners, axis));
        }

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void ShowHighlightHint(bool isTriggered)
        {
            img_highlightForTest.color = isTriggered ?
                Color.red :
                Color.white;
        }

        private Vector3 ClosestPointOnRectangle(Vector3 point, Vector3[] rectCorners)
        {
            Vector3 closestPoint = rectCorners[0];
            float minDistance = float.MaxValue;

            for (int i = 0; i < rectCorners.Length; i++)
            {
                Vector3 edgeStart = rectCorners[i];
                Vector3 edgeEnd = rectCorners[(i + 1) % rectCorners.Length];

                Vector3 closest = ClosestPointOnLineSegment(point, edgeStart, edgeEnd);
                float distance = Vector3.Distance(point, closest);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestPoint = closest;
                }
            }

            return closestPoint;
        }

        private Vector3 ClosestPointOnLineSegment(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            Vector3 line = lineEnd - lineStart;
            float lineLengthSquared = line.sqrMagnitude;
            if (lineLengthSquared == 0) return lineStart;

            float t = Vector3.Dot(point - lineStart, line) / lineLengthSquared;
            t = Mathf.Clamp01(t);

            return lineStart + t * line;
        }
    }
}