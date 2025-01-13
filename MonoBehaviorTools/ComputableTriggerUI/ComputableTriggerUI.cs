using System;
using SNShien.Common.AdapterTools;
using UnityEngine;
using UnityEngine.UI;

namespace SNShien.Common.MonoBehaviorTools
{
    [RequireComponent(typeof(Collider2DAdapterComponent))]
    public class ComputableTriggerUI : MonoBehaviour, ICollider2DHandler
    {
        [SerializeField] private bool isShowHighlightHintForTest;
        [SerializeField] private ComputableUIShapeType shapeType;
        [SerializeField] private Image img_highlightForTest;

        private Collider2DAdapterComponent collider2D;
        private RectTransform rectTransform;
        private ComputableTriggerUI currentTrackingTarget;

        public event Action<bool> OnChangeTriggeredState;
        private bool CurrentTriggeredState { get; set; }

        private ComputableUIShapeType ShapeType => shapeType;
        private RectTransform RectTransform => rectTransform;

        public void ColliderTriggerEnter2D(ICollider2DAdapter col)
        {
        }

        public void ColliderTriggerExit2D(ICollider2DAdapter col)
        {
        }

        public void ColliderTriggerStay2D(ICollider2DAdapter col)
        {
            if (currentTrackingTarget != null)
                return;

            ComputableTriggerUI uiObj = col.GetComponent<ComputableTriggerUI>();
            if (uiObj == null)
                return;

            currentTrackingTarget = uiObj;
            currentTrackingTarget.SetTriggeredState(true);
        }

        public void CollisionEnter2D(ICollision2DAdapter col)
        {
        }

        public void CollisionExit2D(ICollision2DAdapter col)
        {
        }

        private void Update()
        {
            if (currentTrackingTarget == null)
                return;

            bool isInRange = IsInRange(currentTrackingTarget.ShapeType, currentTrackingTarget.RectTransform);
            currentTrackingTarget.SetTriggeredState(isInRange);

            if (isInRange == false)
                currentTrackingTarget = null;
        }

        bool IsInRange(ComputableUIShapeType targetShapeType, RectTransform targetRectTransform)
        {
            Vector3 targetRectCenterPos = targetRectTransform.localPosition;
            float targetCircleRadius = targetRectTransform.sizeDelta.y / 2;
            Vector3 targetRectSize = targetRectTransform.sizeDelta;
            Vector3 targetRectMin = targetRectCenterPos - targetRectSize / 2;
            Vector3 targetRectMax = targetRectCenterPos + targetRectSize / 2;
            Vector3 selfRectCenterPos = RectTransform.localPosition;
            Vector3 selfRectSize = RectTransform.sizeDelta;
            float selfCircleRadius = RectTransform.sizeDelta.y / 2;
            Vector3 selfRectMin = selfRectCenterPos - selfRectSize / 2;
            Vector3 selfRectMax = selfRectCenterPos + selfRectSize / 2;

            if (targetShapeType == ComputableUIShapeType.Circle)
            {
                switch (ShapeType)
                {
                    //對方是圓形, 自己是矩形
                    case ComputableUIShapeType.Rectangle:
                        return Vector3.Distance(targetRectCenterPos, new Vector3(
                            Mathf.Clamp(targetRectCenterPos.x, selfRectMin.x, selfRectMax.x),
                            Mathf.Clamp(targetRectCenterPos.y, selfRectMin.y, selfRectMax.y),
                            0)) <= targetCircleRadius;

                    //對方是圓形, 自己也是圓形
                    case ComputableUIShapeType.Circle:
                        return Vector3.Distance(targetRectCenterPos, selfRectCenterPos) <= targetCircleRadius + selfCircleRadius;
                }
            }
            else if (targetShapeType == ComputableUIShapeType.Rectangle)
            {
                switch (ShapeType)
                {
                    //對方是矩形, 自己也是矩形
                    case ComputableUIShapeType.Rectangle:
                        return selfRectMin.x <= targetRectMax.x && selfRectMax.x >= targetRectMin.x &&
                               selfRectMin.y <= targetRectMax.y && selfRectMax.y >= targetRectMin.y;

                    //對方是矩形, 自己是圓形
                    case ComputableUIShapeType.Circle:
                        return Vector3.Distance(selfRectCenterPos, new Vector3(
                            Mathf.Clamp(selfRectCenterPos.x, targetRectMin.x, targetRectMax.x),
                            Mathf.Clamp(selfRectCenterPos.y, targetRectMin.y, targetRectMax.y),
                            0)) <= selfCircleRadius;
                }
            }

            return false;
        }

        private bool IsShowHighlightHintForTest()
        {
#if !UNITY_EDITOR
     return false;
#endif
            return isShowHighlightHintForTest && img_highlightForTest != null;
        }

        private void SetTriggeredState(bool isTriggered)
        {
            if (CurrentTriggeredState == isTriggered)
                return;

            CurrentTriggeredState = isTriggered;
            OnChangeTriggeredState?.Invoke(CurrentTriggeredState);

            if (IsShowHighlightHintForTest())
                ShowHighlightHint(CurrentTriggeredState);
        }

        private void Awake()
        {
            collider2D = GetComponent<Collider2DAdapterComponent>();
            collider2D.InitHandler(this);

            rectTransform = GetComponent<RectTransform>();
        }

        private void ShowHighlightHint(bool isTriggered)
        {
            img_highlightForTest.color = isTriggered ?
                Color.red :
                Color.white;
        }
    }
}