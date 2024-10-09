using UnityEngine;

namespace SNShien.Common.AdapterTools
{
    public interface IGameObjectSpawner
    {
        GameObject Spawn(GameObject prefab, Transform parent);
    }
}