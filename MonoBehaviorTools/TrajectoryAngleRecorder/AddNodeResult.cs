using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using UnityEngine;

namespace SNShien.Common.MonoBehaviorTools
{
    public partial class TrajectoryAngleRecorder
    {
        public class AddNodeResult
        {
            private readonly List<Vector3> angle3Positions;
            private readonly List<Vector3> angle3LocalPositions;
            private readonly float trajectoryAngle;
            private readonly float idleTime;
            public float CurrentTime { get; }

            public float GetAngle => trajectoryAngle;

            public Vector3 GetLatestLocalPosition => HasLatestLocalPosition ?
                angle3LocalPositions.Last() :
                Vector3.zero;

            public Vector3 GetLatestPosition => HasLatestPosition ?
                angle3Positions.Last() :
                Vector3.zero;

            public bool HasLatestLocalPosition => angle3LocalPositions.Count > 0;
            public bool HasLatestPosition => angle3Positions.Count > 0;
            public bool IsStopped { get; private set; }

            public AddNodeResult(List<Vector3> angle3Positions, List<Vector3> angle3LocalPositions, float trajectoryAngle, float idleTime, float currentTime)
            {
                this.angle3Positions = angle3Positions;
                this.angle3LocalPositions = angle3LocalPositions;
                this.trajectoryAngle = trajectoryAngle;
                this.idleTime = idleTime;
                this.CurrentTime = currentTime;
            }

            public void UpdateStoppedState(float stoppedTimeThreshold)
            {
                IsStopped = idleTime >= stoppedTimeThreshold;
            }

            public bool HasAngle()
            {
                if (IsStopped)
                    return false;
                else
                    return trajectoryAngle >= 0;
            }
        }
    }
}