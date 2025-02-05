using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SNShien.Common.TesterTools;
using UnityEngine;
using UnityEngine.UI;

namespace SNShien.Common.MonoBehaviorTools
{
    public class TrajectoryAngleCalculator : MonoBehaviour
    {
        [SerializeField] private Vector2 passAngleMinAndMax;

        [Header("Debug Hint")] [SerializeField] private bool isTestMode;
        [SerializeField] [ShowIf("isTestMode")] private bool isAutoRecordNode;
        [SerializeField] private Text txt_angle;
        [SerializeField] [ShowIf("isAutoRecordNode")] private float testRecordInterval;

        private List<Vector3> positionNodeList = new List<Vector3>();
        private LineRenderer lineRenderer;
        private float currentAngle;

        private readonly Debugger debugger = new Debugger("TrajectoryAngleCalculator");

        public event Action OnAnglePass;

        private void Start()
        {
            if (isTestMode)
                InitLineRenderer();

            HideAllDebugHint();
        }

        private void InitLineRenderer()
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startColor = Color.yellow;
            lineRenderer.startWidth = 0.1f;
        }

        public void RecordPositionNode()
        {
            AddPositionNode();

            if (positionNodeList.Count == 3)
                CheckSendAnglePassEvent();

            if (isTestMode)
                ShowDebugHint();
        }

        private float GetTrajectoryAngle()
        {
            Vector3 v1 = positionNodeList[1] - positionNodeList[0];
            Vector3 v2 = positionNodeList[2] - positionNodeList[1];

            return Vector3.Angle(v1, v2);
        }

        private void CheckSendAnglePassEvent()
        {
            float trajectoryAngle = GetTrajectoryAngle();
            currentAngle = trajectoryAngle;

            if (isTestMode)
                debugger.ShowLog($"Current Angle: {currentAngle}, Position Node List: {string.Join(",", positionNodeList)}");

            if (CheckAnglePass())
                OnAnglePass?.Invoke();
        }

        private bool CheckAnglePass()
        {
            return currentAngle > passAngleMinAndMax.x && currentAngle < passAngleMinAndMax.y;
        }

        private void AddPositionNode()
        {
            positionNodeList.Add(transform.position);

            if (positionNodeList.Count > 3)
            {
                positionNodeList.RemoveAt(0);
                positionNodeList.TrimExcess();
            }
        }

        private void ClearData()
        {
            positionNodeList = new List<Vector3>();
            currentAngle = 0;
        }

        private IEnumerator Cor_AutoRecordPositionNode()
        {
            while (true)
            {
                yield return new WaitForSeconds(testRecordInterval);

                RecordPositionNode();
            }
        }

        private void ShowDebugHint()
        {
            HideAllDebugHint();
            ShowLine();
            ShowCurrentAngle();
        }

        private void ShowCurrentAngle()
        {
            if (currentAngle == 0 ||
                txt_angle == null)
                return;

            txt_angle.gameObject.SetActive(true);
            txt_angle.text = currentAngle.ToString("0");
            txt_angle.color = CheckAnglePass() ?
                Color.green :
                Color.white;
        }

        private void ShowLine()
        {
            lineRenderer.positionCount = positionNodeList.Count;
            lineRenderer.SetPositions(positionNodeList.ToArray());
        }

        private void HideAllDebugHint()
        {
            if (lineRenderer != null)
                lineRenderer.positionCount = 0;

            if (txt_angle != null)
                txt_angle.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (isAutoRecordNode)
                StartCoroutine(Cor_AutoRecordPositionNode());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            ClearData();
        }
    }
}