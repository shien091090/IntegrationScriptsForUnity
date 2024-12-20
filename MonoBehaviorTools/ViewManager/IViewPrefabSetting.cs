using System;
using System.Collections.Generic;

namespace SNShien.Common.MonoBehaviorTools
{
    public interface IViewPrefabSetting
    {
        List<ArchitectureView> GetPrefabList { get; }
        Dictionary<Type, int> GetViewSortOrderDict();
    }
}