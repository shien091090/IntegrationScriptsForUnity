using UnityEngine;
using Zenject;

namespace SNShien.Common.AdapterTools
{
    public class GameObjectSpawner : IGameObjectSpawner
    {
        private readonly System.Func<GameObject, Transform, GameObject> instantiatePrefabAction;

        public GameObjectSpawner(DiContainer container)
        {
            instantiatePrefabAction = container.InstantiatePrefab;
        }

        public GameObject Spawn(GameObject prefab, Transform parent)
        {
            if (instantiatePrefabAction == null)
                return Object.Instantiate(prefab, parent);
            else
                return instantiatePrefabAction(prefab, parent);
        }
    }
}