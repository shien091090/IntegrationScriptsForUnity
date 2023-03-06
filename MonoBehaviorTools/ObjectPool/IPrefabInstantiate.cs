using UnityEngine;

namespace SNShien.Common.MonoBehaviorTools
{
    public interface IPrefabInstantiate
    {
        GameObject InstantiateGameObject(GameObject prefabReference, Transform parentHolder);
    }
}