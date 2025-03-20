using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using SNShien.Common.TesterTools;
using UnityEngine;
using UnityEngine.UI;

namespace SNShien.Common.MonoBehaviorTools
{
    [RequireComponent(typeof(TrajectoryAngleRecorder))]
    public partial class TrajectoryCheckmarkDetector : MonoBehaviour, ITrajectoryCheckmarkDetector
    {
        [SerializeField] private bool isTestMode;
        [SerializeField] private float recordAngleFreq;
        [SerializeField] private float stoppedTimeThreshold;
        [SerializeField] private float fistLineCheckDistance;
        [SerializeField] private float firstLineMaxErrorThreshold;
        [SerializeField] private float firstLineAverageErrorThreshold;
        [SerializeField] private Vector2 checkmarkAngleMinMax;
        [SerializeField] private float secondLineCheckDistance;
        [SerializeField] private float secondLineCheckAngleTimeDiff;
        [SerializeField] [ShowIf("$isTestMode")] private Text txt_debugHint;

        private readonly Debugger debugger = new Debugger("TrajectoryCheckmarkDetector");

        private TrajectoryAngleRecorder trajectoryAngleRecorder;
        private float timer;
        private IStateMachine currentStateMachine;

        public event Action OnTriggerCheckmark;

        public bool CheckFirstLineDistanceAchieved(Vector3 firstLocalPos, Vector3 lastLocalPos)
        {
            float distance = Vector3.Distance(firstLocalPos, lastLocalPos);
            return distance >= fistLineCheckDistance;
        }

        public bool CheckSecondLineDistanceAchieved(Vector3 firstLocalPos, Vector3 lastLocalPos)
        {
            float distance = Vector3.Distance(firstLocalPos, lastLocalPos);
            return distance >= secondLineCheckDistance;
        }

        public void SendTriggerEvent()
        {
            debugger.ShowLog("Trigger Checkmark");
            OnTriggerCheckmark?.Invoke();
        }

        public void ShowLog(string log)
        {
            debugger.ShowLog(log);
        }

        public bool IsTriggerCheckmarkAngle(float angle)
        {
            return angle > checkmarkAngleMinMax.x && angle < checkmarkAngleMinMax.y;
        }

        public bool IsStraightLine(List<Vector3> localPositionList)
        {
            if (localPositionList.Count < 3) return true;

            float sumX = 0f;
            float sumY = 0f;
            float sumXX = 0f;
            float sumXY = 0f;

            for (int i = 0; i < localPositionList.Count; i++)
            {
                sumX += localPositionList[i].x;
                sumY += localPositionList[i].y;
                sumXX += localPositionList[i].x * localPositionList[i].x;
                sumXY += localPositionList[i].x * localPositionList[i].y;
            }

            float denominator = localPositionList.Count * sumXX - sumX * sumX;
            if (Mathf.Approximately(denominator, 0f))
                return false;

            float slope = (localPositionList.Count * sumXY - sumX * sumY) / denominator;
            float intercept = (sumY - slope * sumX) / localPositionList.Count;

            float totalError = 0f;
            float lastError = 0f;
            
            for (int i = 0; i < localPositionList.Count; i++)
            {
                // float predictedY = slope * localPositionList[i].x + intercept;
                float error = Mathf.Abs(slope *  localPositionList[i].x -   localPositionList[i].y + intercept) / Mathf.Sqrt(slope * slope + 1);
                totalError += error;
                lastError = error;
            }

            float averageError = totalError / localPositionList.Count;
            // debugger.ShowLog($"totalError: {totalError}, averageError: {averageError}, lastError: {lastError}");
            return lastError <= firstLineMaxErrorThreshold && averageError <= firstLineAverageErrorThreshold;
        }

        public bool IsTimeToCheckCheckmarkAngle(float timeDiff)
        {
            return timeDiff >= secondLineCheckAngleTimeDiff;
        }

        public float GetTrajectoryAngle(Vector3 startPos, Vector3 cornerPos, Vector3 endPos)
        {
            return trajectoryAngleRecorder.GetTrajectoryAngle(startPos, cornerPos, endPos);
        }

        public void ShowDebugLine(List<Vector3> debugLinePositionList)
        {
            if (debugLinePositionList == null)
                return;

            if (isTestMode && debugLinePositionList.Count > 0)
                trajectoryAngleRecorder.ShowDebugLine(debugLinePositionList);
        }

        public void ShowDebugAngle(float angle)
        {
            if (isTestMode)
                trajectoryAngleRecorder.ShowDebugAngleHint(angle);
        }

        private void Update()
        {
            RecordPositionNodeAndCheckStateMachine();
            // timer += Time.deltaTime;
            //
            // if (timer >= recordAngleFreq)
            // {
            //     timer -= recordAngleFreq;
            // }
        }

        private void RecordPositionNodeAndCheckStateMachine()
        {
            TrajectoryAngleRecorder.AddNodeResult result = trajectoryAngleRecorder.AddPositionNode();
            result.UpdateStoppedState(stoppedTimeThreshold);

            currentStateMachine.Execute(result, out StateMachineCarryOverInfo nextStateInfo);

            if (nextStateInfo != null &&
                currentStateMachine.Mode != nextStateInfo.TrajectoryMode)
            {
                TrajectoryMode previousMode = currentStateMachine.Mode;
                currentStateMachine = CreateStateMachine(nextStateInfo);
                TrajectoryMode currentMode = currentStateMachine.Mode;

                debugger.ShowLog($"Change State: {previousMode} -> {currentMode}");

                if (isTestMode)
                    txt_debugHint.text = $"{currentMode}";
            }
        }

        private void ClearData()
        {
            timer = 0;
        }

        private IStateMachine CreateStateMachine(StateMachineCarryOverInfo carryOverInfo)
        {
            IStateMachine newStateMachine = null;
            switch (carryOverInfo.TrajectoryMode)
            {
                case TrajectoryMode.Stop:
                    trajectoryAngleRecorder.SetDefaultDebugLineColor();
                    newStateMachine = new StateMachine_Stop();
                    break;

                case TrajectoryMode.FirstLine:
                    trajectoryAngleRecorder.SetDefaultDebugLineColor();
                    newStateMachine = new StateMachine_FirstLine();
                    break;

                case TrajectoryMode.WaitForCheckmark:
                    newStateMachine = new StateMachine_WaitForCheckmark();
                    break;

                case TrajectoryMode.SecondLine:
                    trajectoryAngleRecorder.SetDebugLineColor(Color.yellow);
                    newStateMachine = new StateMachine_SecondLine();
                    break;

                case TrajectoryMode.TriggerEvent:
                    newStateMachine = new StateMachine_TriggerEvent();
                    break;
            }

            newStateMachine.Init(this, carryOverInfo);
            return newStateMachine;
        }

        private void Awake()
        {
            ClearData();
            trajectoryAngleRecorder = GetComponent<TrajectoryAngleRecorder>();
            currentStateMachine = CreateStateMachine(new StateMachineCarryOverInfo(TrajectoryMode.Stop));
        }
    }
}