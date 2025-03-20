using System.Collections.Generic;
using UnityEngine;

namespace SNShien.Common.MonoBehaviorTools
{
    public partial class TrajectoryCheckmarkDetector
    {
        public class StateMachine_FirstLine : IStateMachine
        {
            public TrajectoryMode Mode => TrajectoryMode.FirstLine;

            private readonly List<Vector3> worldPositionRecordList = new List<Vector3>();
            private readonly List<Vector3> localPositionRecordList = new List<Vector3>();

            private ITrajectoryCheckmarkDetector mainDetector;

            public void Init(ITrajectoryCheckmarkDetector mainDetector, StateMachineCarryOverInfo previousInfo)
            {
                this.mainDetector = mainDetector;
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
                    localPositionRecordList.Add(recordResult.GetLatestLocalPosition);

                if (recordResult.HasLatestPosition)
                    worldPositionRecordList.Add(recordResult.GetLatestPosition);

                if (mainDetector.IsStraightLine(localPositionRecordList) == false)
                {
                    if (localPositionRecordList.Count >= 2 && mainDetector.CheckFirstLineDistanceAchieved(localPositionRecordList[0], localPositionRecordList[^1]))
                    {
                        SimplifyWorldPositionRecordList();
                        SimplifyLocalPositionRecordList();

                        nextStateInfo = new StateMachineCarryOverInfo(TrajectoryMode.WaitForCheckmark, worldPositionRecordList, localPositionRecordList);
                    }
                    else
                    {
                        localPositionRecordList.Clear();
                        worldPositionRecordList.Clear();
                    }
                }

                mainDetector.ShowDebugLine(worldPositionRecordList);
            }

            private void SimplifyLocalPositionRecordList()
            {
                Vector3 startPos = localPositionRecordList[0];
                Vector3 endPos = localPositionRecordList[^1];
                localPositionRecordList.Clear();
                localPositionRecordList.Add(startPos);
                localPositionRecordList.Add(endPos);
            }

            private void SimplifyWorldPositionRecordList()
            {
                Vector3 startPos = worldPositionRecordList[0];
                Vector3 endPos = worldPositionRecordList[^1];
                worldPositionRecordList.Clear();
                worldPositionRecordList.Add(startPos);
                worldPositionRecordList.Add(endPos);
            }
        }
    }
}