using System.Collections.Generic;
using UnityEngine;

namespace SNShien.Common.MonoBehaviorTools
{
    public partial class TrajectoryCheckmarkDetector
    {
        public class StateMachine_WaitForCheckmark : IStateMachine
        {
            public TrajectoryMode Mode => TrajectoryMode.WaitForCheckmark;

            private ITrajectoryCheckmarkDetector mainDetector;
            private List<Vector3> worldPositionRecordList = new List<Vector3>();
            private List<Vector3> firstLineLocalPositionRecordList = new List<Vector3>();
            private List<Vector3> firstLinePositionRecordList = new List<Vector3>();
            private List<Vector3> localPositionRecordList = new List<Vector3>();
            private float startTime;

            private bool IsInitStartTime => startTime > 0;

            public void Init(ITrajectoryCheckmarkDetector mainDetector, StateMachineCarryOverInfo previousInfo)
            {
                this.mainDetector = mainDetector;
                firstLinePositionRecordList = previousInfo.CarryOverWorldPositionList ?? new List<Vector3>();
                firstLineLocalPositionRecordList = previousInfo.CarryOverLocalPositionList ?? new List<Vector3>();

                worldPositionRecordList = new List<Vector3>();
                worldPositionRecordList.AddRange(firstLinePositionRecordList);
            }

            public void Execute(TrajectoryAngleRecorder.AddNodeResult recordResult, out StateMachineCarryOverInfo nextStateInfo)
            {
                nextStateInfo = null;

                if (recordResult.IsStopped)
                {
                    nextStateInfo = new StateMachineCarryOverInfo(TrajectoryMode.Stop);
                    return;
                }

                if (recordResult.HasAngle() == false)
                    return;

                if (recordResult.HasLatestPosition)
                    worldPositionRecordList.Add(recordResult.GetLatestPosition);

                if (IsNeedRecordLocalPosition(recordResult))
                    localPositionRecordList.Add(recordResult.GetLatestLocalPosition);

                mainDetector.ShowDebugLine(worldPositionRecordList);

                if (IsInitStartTime == false)
                {
                    startTime = recordResult.CurrentTime;
                    return;
                }

                if (mainDetector.IsTimeToCheckCheckmarkAngle(recordResult.CurrentTime - startTime) == false)
                    return;

                float angle = GetCheckmarkAngle(recordResult);
                mainDetector.ShowDebugAngle(angle);
                if (mainDetector.IsTriggerCheckmarkAngle(angle))
                {
                    SimplifyWorldPositionRecordList();
                    localPositionRecordList.Add(recordResult.GetLatestLocalPosition);
                    nextStateInfo = new StateMachineCarryOverInfo(TrajectoryMode.SecondLine, worldPositionRecordList, localPositionRecordList);
                }
                else
                {
                    nextStateInfo = new StateMachineCarryOverInfo(TrajectoryMode.FirstLine);
                }
            }

            private bool IsNeedRecordLocalPosition(TrajectoryAngleRecorder.AddNodeResult recordResult)
            {
                return localPositionRecordList.Count == 0;
            }

            private float GetCheckmarkAngle(TrajectoryAngleRecorder.AddNodeResult recordResult)
            {
                List<Vector3> angle3LocalPositionList = new List<Vector3>
                {
                    firstLineLocalPositionRecordList[0],
                    firstLineLocalPositionRecordList[^1],
                    recordResult.GetLatestLocalPosition
                };

                float angle = mainDetector.GetTrajectoryAngle(angle3LocalPositionList[0], angle3LocalPositionList[1], angle3LocalPositionList[2]);
                return angle;
            }

            private void SimplifyWorldPositionRecordList()
            {
                Vector3 startPos = firstLinePositionRecordList[0];
                Vector3 cornerPos = firstLinePositionRecordList[^1];
                Vector3 endPos = worldPositionRecordList[^1];

                worldPositionRecordList.Clear();
                worldPositionRecordList.Add(startPos);
                worldPositionRecordList.Add(cornerPos);
                worldPositionRecordList.Add(endPos);
            }
        }
    }
}