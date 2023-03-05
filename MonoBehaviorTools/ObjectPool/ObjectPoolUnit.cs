using System.Collections.Generic;
using UnityEngine;

namespace SNShien.Common.MonoBehaviorTools
{
    [System.Serializable]
    public class ObjectPoolUnit //物件池元素
    {
        public string gameObjectName; //遊戲物件名稱
        public GameObject prefabReference; //預置體參考
        public Transform parentHolder; //父物件
        public List<GameObject> objectPoolList; //物件池列表

        //加入遊戲物件至列表
        public void AddElement(GameObject go)
        {
            if (objectPoolList == null)
                objectPoolList = new List<GameObject>();
        
            objectPoolList.Add(go);
        }
    }
}