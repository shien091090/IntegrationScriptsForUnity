#if CUSTOM_USING_ODIN
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SNShien.Common.MonoBehaviorTools
{
    [CreateAssetMenu(fileName = "ViewPrefabSetting", menuName = "SNShien/Create ViewPrefabSetting")]
    public class ViewPrefabScriptableObject : ScriptableObject, IViewPrefabSetting
    {
        [SerializeField] private List<ArchitectureView> prefabList;

        public List<ArchitectureView> GetPrefabList => prefabList;

        public Dictionary<Type, int> GetViewSortOrderDict()
        {
            Dictionary<Type, int> result = new Dictionary<Type, int>();

            List<ArchitectureView> prefabList = new List<ArchitectureView>();
            prefabList.AddRange(GetPrefabList);

            prefabList.Reverse();
            int sortOrder = 0;
            foreach (ArchitectureView viewPrefab in prefabList)
            {
                result[viewPrefab.GetType()] = sortOrder;
                sortOrder += 10;
            }

            return result;
        }
    }
}
#endif