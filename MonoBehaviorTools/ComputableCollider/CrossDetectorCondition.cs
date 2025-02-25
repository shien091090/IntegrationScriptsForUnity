using System;
using UnityEngine;

namespace SNShien.Common.MonoBehaviorTools
{
    public partial class ComputableColliderCrossDetector
    {
        [System.Serializable]
        public class CrossDetectorCondition
        {
            [SerializeField] private string key;
            [SerializeField] private float enterAngleBase;
            [SerializeField] private float enterAngleRange;
            [SerializeField] private float exitAngleBase;
            [SerializeField] private float exitAngleRange;

            public string Key => key;

            public bool CheckEnterAngle(float angle)
            {
                return Math.Abs(angle) >= enterAngleBase - enterAngleRange && Math.Abs(angle) <= enterAngleBase + enterAngleRange;
            }

            public bool CheckExitAngle(float angle)
            {
                return Math.Abs(angle) >= exitAngleBase - exitAngleRange && Math.Abs(angle) <= exitAngleBase + exitAngleRange;
            }
        }
    }
}