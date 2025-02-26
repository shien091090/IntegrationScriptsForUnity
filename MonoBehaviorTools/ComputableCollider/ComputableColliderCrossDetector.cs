using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SNShien.Common.TesterTools;
using UnityEngine;
using UnityEngine.UI;

namespace SNShien.Common.MonoBehaviorTools
{
    [RequireComponent(typeof(ComputableCollider))]
    public partial class ComputableColliderCrossDetector : SerializedMonoBehaviour
    {
        [SerializeField] private CrossDetectorCondition[] crossDetectorConditions;

        [Header("Debug Hint")] [SerializeField] private bool showDebugHint;
        [SerializeField] private GameObject go_enterHint;
        [SerializeField] private GameObject go_exitHint;
        [SerializeField] private Text txt_enterPosAngle;
        [SerializeField] private Text txt_exitPosAngle;
        [SerializeField] private Dictionary<string, GameObject> crossSuccessHintDict;
        [SerializeField] private GameObject go_crossFailHint;
        private bool isStartCross;
        private CrossDetectorCondition currentMatchCondition;

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

        private void TrySetCrossSuccessHintOpen(string key)
        {
            if (crossSuccessHintDict.ContainsKey(key))
                crossSuccessHintDict[key].gameObject.SetActive(true);
        }

        private float GetAngle(Vector3 targetPos)
        {
            Vector3 targetDir = targetPos - transform.position;
            float angle = Vector3.SignedAngle(targetDir, transform.right, Vector3.forward);

            if (angle < 0)
                angle = 360 + angle;

            return angle;
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

        private bool CheckEnterAngleConditions(Vector3 targetPos, out CrossDetectorCondition match)
        {
            match = null;
            float angle = GetAngle(targetPos);

            SetDebugEnterPosHint(targetPos, angle.ToString("0"));

            foreach (CrossDetectorCondition condition in crossDetectorConditions)
            {
                if (condition.CheckEnterAngle(angle))
                    match = condition;
            }

            return match != null;
        }

        private bool CheckExitAngleCondition(Vector3 targetPos)
        {
            float angle = GetAngle(targetPos);

            SetDebugExitPosHint(targetPos, angle.ToString("0"));

            return currentMatchCondition != null && currentMatchCondition.CheckExitAngle(angle);
        }

        private void ClearData()
        {
            isStartCross = false;
            currentMatchCondition = null;
        }

        private void ShowDebugCrossSuccessHint(CrossDetectorCondition conditionInfo)
        {
            if (IsShowDebugHint() == false)
                return;

            HideAllSuccessHint();
            TrySetCrossSuccessHintOpen(conditionInfo.Key);
            go_crossFailHint.SetActive(false);
        }

        private void ShowDebugCrossFailHint()
        {
            if (IsShowDebugHint() == false)
                return;

            HideAllSuccessHint();
            go_crossFailHint.SetActive(true);
        }

        private void HideAllSuccessHint()
        {
            foreach (GameObject go in crossSuccessHintDict.Values)
            {
                go.SetActive(false);
            }
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
            HideAllSuccessHint();
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
                if (CheckEnterAngleConditions(target.Position, out CrossDetectorCondition match))
                {
                    isStartCross = true;
                    currentMatchCondition = match;
                }
                else
                    ShowDebugCrossFailHint();
            }
            else
            {
                bool checkExitAngle = CheckExitAngleCondition(target.Position);
                if (isStartCross)
                {
                    if (checkExitAngle)
                    {
                        ShowDebugCrossSuccessHint(currentMatchCondition);

                        OnTriggerCross?.Invoke(target.gameObject);
                    }
                    else
                        ShowDebugCrossFailHint();
                }

                currentMatchCondition = null;
                isStartCross = false;
            }
        }
    }
}