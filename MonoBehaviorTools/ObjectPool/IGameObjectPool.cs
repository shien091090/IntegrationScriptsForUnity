using UnityEngine;

namespace SNShien.Common.MonoBehaviorTools
{
    public interface IGameObjectPool
    {
        void SpawnGameObject(string prefabName, Vector3 position = default, Vector3 scale = default);
    }
}