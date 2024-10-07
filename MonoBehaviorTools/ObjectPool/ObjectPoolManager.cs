using System.Collections.Generic;
using GameCore;
using SNShien.Common.ProcessTools;
using UnityEngine;

namespace SNShien.Common.MonoBehaviorTools
{
    public class ObjectPoolManager : MonoBehaviour, IGameObjectPool
    {
        [SerializeField] private ZenjectGameObjectSpawner zenjectGameObjectSpawner;
        [SerializeField] private List<ObjectPoolUnit> objectPoolSetting; //物件池設定

        private Dictionary<string, ObjectPoolUnit> objectPoolTagDict; //(字典)從物件名稱查找ObjectPoolUnit

        private bool IsUseZenjectGameObjectSpawner => zenjectGameObjectSpawner != null;

        public GameObject SpawnGameObject(string prefabName, Vector3 position = default, Vector3 scale = default)
        {
            if (objectPoolTagDict.ContainsKey(prefabName) == false)
                return null;

            CheckAutoCreateHolder(prefabName);
            GameObject go = PickUpObject(prefabName);

            if (position != default)
                go.transform.position = position;

            if (scale != default)
                go.transform.localScale = scale;

            go.SetActive(true);
            return go;
        }

        public T SpawnGameObject<T>(string prefabName, Vector3 position = default, Vector3 scale = default) where T : Component
        {
            GameObject go = SpawnGameObject(prefabName, position, scale);
            return go.GetComponent<T>();
        }

        public void ExecuteModelInit()
        {
            Init();
        }

        private void Awake()
        {
            Init();
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
                GameObject go = IsUseZenjectGameObjectSpawner ?
                    zenjectGameObjectSpawner.Spawn(u.prefabReference, u.parentHolder) :
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
    }
}