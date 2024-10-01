using UnityEngine;
using Zenject;

namespace SNShien.Common.ProcessTools
{
    public class ZenjectGameObjectSpawner : MonoInstaller
    {
        public GameObject Spawn(GameObject prefab, Transform parent)
        {
            return Container.InstantiatePrefab(prefab, parent);
        }
    }
}