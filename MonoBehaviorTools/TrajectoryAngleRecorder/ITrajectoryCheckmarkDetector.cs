using System.Collections.Generic;
using UnityEngine;

namespace SNShien.Common.MonoBehaviorTools
{
    public interface ITrajectoryCheckmarkDetector
    {
        bool IsTriggerCheckmarkAngle(float angle);
        bool IsStraightLine(List<Vector3> localPositionList);
        bool IsTimeToCheckCheckmarkAngle(float timeDiff);
        float GetTrajectoryAngle(Vector3 startPos, Vector3 cornerPos, Vector3 endPos);
        bool CheckFirstLineDistanceAchieved(Vector3 firstLocalPos, Vector3 lastLocalPos);
        bool CheckSecondLineDistanceAchieved(Vector3 firstLocalPos, Vector3 lastLocalPos);
        void SendTriggerEvent();
        void ShowLog(string log);
        void ShowDebugLine(List<Vector3> debugLinePositionList);
        void ShowDebugAngle(float angle);
    }
}