using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SNShien.Common.TesterTools;
using UnityEngine;
using UnityEngine.UI;

namespace SNShien.Common.MonoBehaviorTools
{
    public partial class TrajectoryAngleRecorder : MonoBehaviour
    {
        [Header("Debug Hint")] [SerializeField] private bool isTestMode;
        [SerializeField] [ShowIf("$isTestMode")] private bool isAutoRecord;
        [SerializeField] [ShowIf("$isTestMode")] private Text debugAngleText;
        [SerializeField] [ShowIf("$isAutoRecord")] private float recordNodeFreq;

        private LineRenderer lineRenderer;
        private List<Vector3> positionNodeList = new List<Vector3>();
        private List<Vector3> localPositionNodeList = new List<Vector3>();

        private readonly Debugger debugger = new Debugger("TrajectoryAngleCalculator");
        private float triggerRecordNodeTimer;
        private float totalTimer;
        private float recordIdleTime;
        private Vector3 lastRecordPos;
        private List<Vector3> allPositionRecordList = new List<Vector3>();
        private Material debugLineMaterial;
        private Color originDebugLineColor;

        private void Update()
        {
            totalTimer += Time.deltaTime;

            if (isTestMode && isAutoRecord)
            {
                triggerRecordNodeTimer += Time.deltaTime;

                if (triggerRecordNodeTimer >= recordNodeFreq)
                {
                    AddPositionNode();
                    triggerRecordNodeTimer -= recordNodeFreq;
                }
            }
        }

        private void Start()
        {
            ClearData();

            if (isTestMode)
                InitLineRenderer();

            HideAllDebugHint();
        }

        private void InitLineRenderer()
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.1f;

            originDebugLineColor = Color.gray;
            debugLineMaterial = new Material(Shader.Find("UI/Default"))
            {
                color = originDebugLineColor
            };

            lineRenderer.material = debugLineMaterial;
        }

        public float GetTrajectoryAngle(Vector3 startPos, Vector3 cornerPos, Vector3 endPos)
        {
            Vector3 v1 = cornerPos - startPos;
            Vector3 v2 = endPos - cornerPos;

            return Vector3.Angle(v1, v2);
        }

        public void SetDebugLineColor(Color newColor)
        {
            if (debugLineMaterial != null)
                debugLineMaterial.color = newColor;
        }

        public void SetDefaultDebugLineColor()
        {
            SetDebugLineColor(originDebugLineColor);
        }

        public AddNodeResult AddPositionNode()
        {
            RecordAngle3Pos(out float trajectoryAngle);
            RecordIdleTime(out float idleTime);

            // if (isTestMode)
            // {
            //     HideAllDebugHint();
            //     
            //     allPositionRecordList.Add(transform.position);
            //     ShowDebugLine(allPositionRecordList);
            //
            //     if (trajectoryAngle >= 0)
            //         ShowCurrentAngle(trajectoryAngle);
            // }

            AddNodeResult result = new AddNodeResult(positionNodeList, localPositionNodeList, trajectoryAngle, idleTime, totalTimer);
            return result;
        }

        public void ClearData()
        {
            positionNodeList = new List<Vector3>();
            localPositionNodeList = new List<Vector3>();
            allPositionRecordList = new List<Vector3>();
            lastRecordPos = Vector3.zero;
            recordIdleTime = 0;
            triggerRecordNodeTimer = 0;
            totalTimer = 0;
        }

        public void ShowDebugLine(List<Vector3> posList)
        {
            lineRenderer.positionCount = posList.Count;
            lineRenderer.SetPositions(posList.ToArray());
        }

        public void ShowDebugAngleHint(float angle)
        {
            if (angle == 0)
                return;

            debugAngleText.transform.SetParent(transform.parent);
            debugAngleText.transform.position = transform.position;
            debugAngleText.gameObject.SetActive(true);
            debugAngleText.text = angle.ToString("0");
        }

        public void HideDebugAngleHint()
        {
            if (debugAngleText != null)
                debugAngleText.gameObject.SetActive(false);
        }

        private float GetTrajectoryAngle()
        {
            return GetTrajectoryAngle(positionNodeList[0], positionNodeList[1], positionNodeList[2]);
        }

        private void RecordIdleTime(out float idleTime)
        {
            idleTime = -1;

            if (positionNodeList.Count == 0)
                return;

            Vector3 newPos = positionNodeList[^1];

            bool isPosChanged = lastRecordPos == Vector3.zero || newPos != lastRecordPos;
            if (isPosChanged)
                recordIdleTime = totalTimer;

            lastRecordPos = newPos;

            idleTime = totalTimer - recordIdleTime;
        }

        private void RecordAngle3Pos(out float trajectoryAngle)
        {
            positionNodeList.Add(transform.position);
            localPositionNodeList.Add(transform.localPosition);

            if (positionNodeList.Count > 3)
            {
                positionNodeList.RemoveAt(0);
                positionNodeList.TrimExcess();
            }

            if (localPositionNodeList.Count > 3)
            {
                localPositionNodeList.RemoveAt(0);
                localPositionNodeList.TrimExcess();
            }

            trajectoryAngle = -1;
            if (positionNodeList.Count == 3)
                trajectoryAngle = GetTrajectoryAngle();
        }

        private void HideAllDebugHint()
        {
            if (lineRenderer != null)
                lineRenderer.positionCount = 0;

            if (debugAngleText != null)
                debugAngleText.gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            ClearData();
        }
    }
}