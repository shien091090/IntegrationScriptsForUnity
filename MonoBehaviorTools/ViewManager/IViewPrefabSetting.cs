using System.Collections.Generic;
using UnityEngine;

namespace SNShien.Common.MonoBehaviorTools
{
    public interface IViewPrefabSetting
    {
        List<GameObject> GetPrefabList { get; }
    }
}