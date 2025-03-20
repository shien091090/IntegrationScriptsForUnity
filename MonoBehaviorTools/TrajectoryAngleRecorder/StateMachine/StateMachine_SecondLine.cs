using System.Collections.Generic;
using UnityEngine;

namespace SNShien.Common.MonoBehaviorTools
{
    public partial class TrajectoryCheckmarkDetector
    {
        public class StateMachine_SecondLine : IStateMachine
        {
            public TrajectoryMode Mode => TrajectoryMode.SecondLine;

            private ITrajectoryCheckmarkDetector mainDetector;
            private List<Vector3> secondLineLocalPositionRecordList;
            private List<Vector3> worldPositionRecordList;

            public void Init(ITrajectoryCheckmarkDetector mainDetector, StateMachineCarryOverInfo previousInfo)
            {
                this.mainDetector = mainDetector;
                worldPositionRecordList = previousInfo.CarryOverWorldPositionList ?? new List<Vector3>();
                secondLineLocalPositionRecordList = previousInfo.CarryOverLocalPositionList ?? new List<Vector3>();
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

                if (recordResult.HasLatestLocalPosition)
                    secondLineLocalPositionRecordList.Add(recordResult.GetLatestLocalPosition);

                if (recordResult.HasLatestPosition)
                    worldPositionRecordList.Add(recordResult.GetLatestPosition);

                if (mainDetector.IsStraightLine(secondLineLocalPositionRecordList))
                {
                    if (secondLineLocalPositionRecordList.Count >= 2 &&
                        mainDetector.CheckSecondLineDistanceAchieved(secondLineLocalPositionRecordList[0], secondLineLocalPositionRecordList[^1]))
                    {
                        nextStateInfo = new StateMachineCarryOverInfo(TrajectoryMode.TriggerEvent);
                    }
                }
                else
                {
                    nextStateInfo = new StateMachineCarryOverInfo(TrajectoryMode.FirstLine);
                }

                mainDetector.ShowDebugLine(worldPositionRecordList);
            }
        }
    }
}