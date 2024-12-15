using SNShien.Common.ProcessTools;
using UnityEngine;

namespace SNShien.Common.MonoBehaviorTools
{
    public interface IGameObjectPool
    {
        GameObject SpawnGameObject(string prefabName);
        T SpawnGameObjectAndSetPosition<T>(string prefabName, Vector3 position, TransformType transformType) where T : Component;
        GameObject SpawnGameObjectAndSetPosition(string prefabName, Vector3 position, TransformType transformType);
    }
}