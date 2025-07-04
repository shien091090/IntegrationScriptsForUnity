﻿#if CUSTOM_USING_ZENJECT
using System.Collections.Generic;
using SNShien.Common.AdapterTools;
using UnityEngine;
using Zenject;

namespace SNShien.Common.MonoBehaviorTools
{
    public class ObjectPoolManager : MonoBehaviour, IGameObjectPool
    {
        [InjectOptional] private IGameObjectSpawner gameObjectSpawner;

        [SerializeField] private List<ObjectPoolUnit> objectPoolSetting; //物件池設定

        private Dictionary<string, ObjectPoolUnit> objectPoolTagDict; //(字典)從物件名稱查找ObjectPoolUnit

        public GameObject SpawnGameObject(string prefabName)
        {
            if (objectPoolTagDict.ContainsKey(prefabName) == false)
                return null;

            CheckAutoCreateHolder(prefabName);
            GameObject go = PickUpObject(prefabName);

            go.SetActive(true);
            return go;
        }

        public T SpawnGameObjectAndSetPosition<T>(string prefabName, Vector3 position, TransformType transformType) where T : Component
        {
            GameObject go = SpawnGameObjectAndSetPosition(prefabName, position, transformType);
            return go.GetComponent<T>();
        }

        private void Init()
        {
            if (objectPoolTagDict != null)
                return;

            objectPoolTagDict = new Dictionary<string, ObjectPoolUnit>();

            for (int i = 0; i < objectPoolSetting.Count; i++)
            {
                if (objectPoolTagDict.ContainsKey(objectPoolSetting[i].gameObjectName)) throw new System.Exception("[ERROR]物件名稱重複");

                objectPoolTagDict.Add(objectPoolSetting[i].gameObjectName, objectPoolSetting[i]); //建立字典
            }
        }

        public GameObject SpawnGameObjectAndSetPosition(string prefabName, Vector3 position, TransformType transformType)
        {
            GameObject go = SpawnGameObject(prefabName);
            switch (transformType)
            {
                case TransformType.World:
                    go.transform.position = position;
                    break;

                case TransformType.Local:
                    go.transform.localPosition = position;
                    break;
            }

            return go;
        }

        //從物件池中取得指定物件
        public GameObject PickUpObject(string goName)
        {
            if (objectPoolTagDict.ContainsKey(goName) == false)
                return null;

            GameObject _result = null;
            ObjectPoolUnit _unit = objectPoolTagDict[goName];

            //創立新物件(Lambda)
            System.Action<ObjectPoolUnit> CreateNew = (ObjectPoolUnit u) =>
            {
                GameObject go = gameObjectSpawner != null ?
                    gameObjectSpawner.Spawn(u.prefabReference, u.parentHolder) :
                    Instantiate(u.prefabReference, u.parentHolder);

                u.AddElement(go);

                _result = go;
            };

            if (_unit.ObjectPoolList == null || _unit.ObjectPoolList.Count == 0) //若物件池中無物件
            {
                CreateNew(_unit); //創立新物件
            }
            else //若物件池中存在物件
            {
                for (int i = 0; i < _unit.ObjectPoolList.Count; i++) //尋找隱藏中(active = false)物件
                {
                    if (!_unit.ObjectPoolList[i].activeSelf)
                    {
                        _result = _unit.ObjectPoolList[i];
                        break;
                    }
                }

                if (_result == null) CreateNew(_unit); //若所有物件皆在非隱藏狀態, 則建立新物件
            }

            return _result;
        }

        public void HideAll(string prefabName)
        {
            if (objectPoolTagDict.TryGetValue(prefabName, out ObjectPoolUnit unit) == false)
                return;

            if (unit.ObjectPoolList == null || unit.ObjectPoolList.Count == 0)
                return;

            foreach (GameObject go in unit.ObjectPoolList)
            {
                go.SetActive(false);
            }
        }

        private void CheckAutoCreateHolder(string prefabName)
        {
            if (objectPoolTagDict.TryGetValue(prefabName, out ObjectPoolUnit unit) == false)
                return;

            if (unit.parentHolder != null)
                return;

            unit.parentHolder = new GameObject(prefabName + "Holder").transform;
            unit.parentHolder.parent = transform;
        }

        private void Awake()
        {
            Init();
        }
    }

    public enum TransformType
    {
        World,
        Local
    }
}
#endif