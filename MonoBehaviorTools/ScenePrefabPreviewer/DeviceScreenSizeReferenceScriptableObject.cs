using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SNShien.Common.MonoBehaviorTools
{
    [CreateAssetMenu(fileName = "DeviceScreenSizeReferenceSetting", menuName = "SNShien/Create DeviceScreenSizeReferenceSetting")]
    public class DeviceScreenSizeReferenceScriptableObject : ScriptableObject
    {
        [SerializeField] private DeviceScreenSizeDefine[] defineList;
        public bool IsSettingEmpty => defineList == null || defineList.Length == 0;

        public List<string> GetDeviceTypeStringList => IsSettingEmpty ?
            new List<string>() :
            defineList.Select(x => x.DeviceType).ToList();

        public Vector2Int GetScreenSize(string deviceType)
        {
            if (IsSettingEmpty)
                return Vector2Int.zero;

            DeviceScreenSizeDefine define = defineList.FirstOrDefault(x => x.DeviceType == deviceType);
            return define?.ScreenSize ?? Vector2Int.zero;
        }
    }
}