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
                return CheckAngle(angle, enterAngleBase, enterAngleRange);
            }

            public bool CheckExitAngle(float angle)
            {
                return CheckAngle(angle, exitAngleBase, exitAngleRange);
            }

            private bool CheckAngle(float angle, float angleBase, float passRange)
            {
                float passMin = angleBase - passRange;
                float passMax = angleBase + passRange;

                if (angle >= passMin && angle <= passMax)
                    return true;

                if (passMin < 0)
                {
                    if (angle >= 360 + passMin && angle <= 360)
                        return true;
                }

                if (passMax > 360)
                {
                    if (angle >= 0 && angle <= passMax - 360)
                        return true;
                }

                return false;
            }
        }
    }
}