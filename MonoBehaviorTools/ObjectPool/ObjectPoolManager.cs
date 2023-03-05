using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SNShien.Common.MonoBehaviorTools
{
    public class ObjectPoolManager : MonoBehaviour
    {
        public List<ObjectPoolUnit> objectPoolSetting; //物件池設定
        private Dictionary<string, ObjectPoolUnit> ObjectPoolTagDict { set; get; } //(字典)從物件名稱查找ObjectPoolUnit

        private void Start()
        {
            if (objectPoolSetting == null || objectPoolSetting.Count <= 0)
                return;

            ObjectPoolTagDict = new Dictionary<string, ObjectPoolUnit>();

            foreach (ObjectPoolUnit unit in objectPoolSetting)
            {
                if (ObjectPoolTagDict.ContainsKey(unit.gameObjectName))
                    throw new System.Exception("[ERROR]物件名稱重複");

                ObjectPoolTagDict.Add(unit.gameObjectName, unit); //建立字典
            }
        }

        //從物件池中取得指定物件
        public T PickUpObject<T>(string prefabKey)
        {
            if (!ObjectPoolTagDict.ContainsKey(prefabKey))
                return default; //查無物件

            GameObject resultObj = null;
            ObjectPoolUnit unit = ObjectPoolTagDict[prefabKey];

            if (unit.objectPoolList == null || unit.objectPoolList.Count == 0) //若物件池中無物件
            {
                resultObj = CreateNewObject(unit);
            }
            else //若物件池中存在物件
            {
                foreach (GameObject validObj in unit.objectPoolList.Where(x => !x.activeSelf))
                {
                    resultObj = validObj;
                    break;
                }

                if (resultObj == null)
                    resultObj = CreateNewObject(unit); //若所有物件皆在非隱藏狀態, 則建立新物件
            }

            return resultObj.GetComponent<T>();
        }

        private GameObject CreateNewObject(ObjectPoolUnit unit)
        {
            GameObject newObj = Instantiate(unit.prefabReference, unit.parentHolder);
            unit.AddElement(newObj);

            return newObj;
        }
    }
}