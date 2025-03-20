using System.Collections.Generic;
using UnityEngine;

namespace SNShien.Common.MonoBehaviorTools
{
    public partial class TrajectoryCheckmarkDetector
    {
        public class StateMachineCarryOverInfo
        {
            public TrajectoryMode TrajectoryMode { get; }
            public List<Vector3> CarryOverWorldPositionList { get; }
            public List<Vector3> CarryOverLocalPositionList { get; }

            public StateMachineCarryOverInfo(TrajectoryMode trajectoryMode, List<Vector3> carryOverWorldPositionList = null, List<Vector3> carryOverLocalPositionList = null)
            {
                TrajectoryMode = trajectoryMode;
                CarryOverWorldPositionList = carryOverWorldPositionList;
                CarryOverLocalPositionList = carryOverLocalPositionList;
            }
        }
    }
}