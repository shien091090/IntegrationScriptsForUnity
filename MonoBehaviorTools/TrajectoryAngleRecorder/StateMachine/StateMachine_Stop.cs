using System.Collections.Generic;
using UnityEngine;

namespace SNShien.Common.MonoBehaviorTools
{
    public partial class TrajectoryCheckmarkDetector
    {
        public class StateMachine_Stop : IStateMachine
        {
            public TrajectoryMode Mode => TrajectoryMode.Stop;
            private ITrajectoryCheckmarkDetector mainDetector;
            public List<Vector3> DebugLinePositionList => new List<Vector3>();

            public void Init(ITrajectoryCheckmarkDetector mainDetector, StateMachineCarryOverInfo previousInfo)
            {
                this.mainDetector = mainDetector;
            }

            public void Execute(TrajectoryAngleRecorder.AddNodeResult recordResult, out StateMachineCarryOverInfo nextStateInfo)
            {
                nextStateInfo = new StateMachineCarryOverInfo(recordResult.IsStopped ?
                    Mode :
                    TrajectoryMode.FirstLine);
            }
        }
    }
}