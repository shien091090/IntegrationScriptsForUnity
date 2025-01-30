using System;
using SNShien.Common.TesterTools;
using UnityEngine;
using UnityEngine.UI;

namespace SNShien.Common.MonoBehaviorTools
{
    [RequireComponent(typeof(ComputableCollider))]
    public class ComputableColliderCrossDetector : MonoBehaviour
    {
        [SerializeField] private float enterAngleBase;
        [SerializeField] private float enterAngleRange;
        [SerializeField] private float exitAngleBase;
        [SerializeField] private float exitAngleRange;

        [Header("Debug Hint")] [SerializeField] private bool showDebugHint;
        [SerializeField] private GameObject go_enterHint;
        [SerializeField] private GameObject go_exitHint;
        [SerializeField] private Text txt_enterPosAngle;
        [SerializeField] private Text txt_exitPosAngle;
        [SerializeField] private GameObject go_crossSuccessHint;
        [SerializeField] private GameObject go_crossFailHint;
        private bool isStartCross;

        private readonly Debugger debugger = new Debugger("ComputableColliderCrossDetector");

        public event Action<GameObject> OnTriggerCross;

        public ComputableCollider ComputableCollider { get; private set; }

        private bool IsShowDebugHint()
        {
#if !UNITY_EDITOR
            return false;
#endif
            return showDebugHint;
        }

        private void SetEventRegister(bool isListen)
        {
            ComputableCollider.OnChangeTriggeredState -= OnChangeTriggeredState;

            if (isListen)
            {
                ComputableCollider.OnChangeTriggeredState += OnChangeTriggeredState;
            }
        }

        private void SetDebugExitPosHint(Vector3 pos, string angleText)
        {
            if (IsShowDebugHint() == false)
                return;

            go_exitHint.SetActive(true);
            go_exitHint.transform.position = pos;

            txt_exitPosAngle.gameObject.SetActive(true);
            txt_exitPosAngle.text = angleText;
        }

        private void SetDebugEnterPosHint(Vector3 pos, string angleText)
        {
            if (IsShowDebugHint() == false)
                return;

            go_enterHint.SetActive(true);
            go_enterHint.transform.position = pos;

            txt_enterPosAngle.gameObject.SetActive(true);
            txt_enterPosAngle.text = angleText;
        }

        private void SetDebugCrossSuccessHint(bool isSuccess)
        {
            if (IsShowDebugHint() == false)
                return;

            go_crossSuccessHint.SetActive(isSuccess);
            go_crossFailHint.SetActive(!isSuccess);
        }

        private bool CheckEnterAngle(Vector3 targetPos)
        {
            Vector3 targetDir = targetPos - transform.position;
            float angle = Vector3.SignedAngle(targetDir, transform.right, Vector3.forward);

            SetDebugEnterPosHint(targetPos, angle.ToString("0"));

            return Math.Abs(angle) >= enterAngleBase - enterAngleRange && Math.Abs(angle) <= enterAngleBase + enterAngleRange;
        }

        private bool CheckExitAngle(Vector3 targetPos)
        {
            Vector3 targetDir = targetPos - transform.position;
            float angle = Vector3.SignedAngle(targetDir, transform.right, Vector3.forward);

            SetDebugExitPosHint(targetPos, angle.ToString("0"));

            return Math.Abs(angle) >= exitAngleBase - exitAngleRange && Math.Abs(angle) <= exitAngleBase + exitAngleRange;
        }

        private void ClearData()
        {
            isStartCross = false;
        }

        private void Awake()
        {
            ComputableCollider = GetComponent<ComputableCollider>();
            HideAllHint();
            ClearData();
        }

        private void HideAllHint()
        {
            txt_enterPosAngle.gameObject.SetActive(false);
            txt_exitPosAngle.gameObject.SetActive(false);
            go_enterHint.SetActive(false);
            go_exitHint.SetActive(false);
            go_crossFailHint.SetActive(false);
            go_crossSuccessHint.SetActive(false);
        }

        private void OnEnable()
        {
            SetEventRegister(true);
        }

        private void OnChangeTriggeredState(bool isTriggered, ComputableCollider target)
        {
            if (isTriggered)
            {
                HideAllHint();
                if (CheckEnterAngle(target.Position))
                    isStartCross = true;
                else
                    SetDebugCrossSuccessHint(false);
            }
            else
            {
                bool checkExitAngle = CheckExitAngle(target.Position);
                if (isStartCross)
                {
                    if (checkExitAngle)
                    {
                        SetDebugCrossSuccessHint(true);

                        OnTriggerCross?.Invoke(target.gameObject);
                    }
                    else
                        SetDebugCrossSuccessHint(false);
                }

                isStartCross = false;
            }
        }
    }
}