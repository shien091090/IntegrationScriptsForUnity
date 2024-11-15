using System.Collections.Generic;
using UnityEngine;

namespace SNShien.Common.MonoBehaviorTools
{
    [CreateAssetMenu(fileName = "ViewPrefabSetting", menuName = "SNShien/Create ViewPrefabSetting")]
    public class ViewPrefabScriptableObject : ScriptableObject, IViewPrefabSetting
    {
        [SerializeField] private List<ArchitectureView> prefabList;

        public List<ArchitectureView> GetPrefabList => prefabList;
    }
}