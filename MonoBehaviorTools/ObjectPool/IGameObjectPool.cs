using SNShien.Common.ProcessTools;
using UnityEngine;

namespace SNShien.Common.MonoBehaviorTools
{
    public interface IGameObjectPool : IArchitectureModel
    {
        GameObject SpawnGameObject(string prefabName, Vector3 position = default, Vector3 scale = default);
        T SpawnGameObject<T>(string prefabName, Vector3 position = default, Vector3 scale = default) where T : Component;
    }
}