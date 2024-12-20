using UnityEngine;

namespace SNShien.Common.MonoBehaviorTools
{
    [System.Serializable]
    public class DeviceScreenSizeDefine
    {
        [SerializeField] private string deviceType;
        [SerializeField] private Vector2Int screenSize;

        public string DeviceType => deviceType;
        public Vector2Int ScreenSize => screenSize;
    }
}