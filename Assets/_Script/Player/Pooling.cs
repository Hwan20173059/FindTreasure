using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pooling : MonoBehaviour
{
    [System.Serializable]
    public struct Pool
    {
        public string name;
        public GameObject prefeb;
        public int amount;
    }

    Dictionary<string, Queue<GameObject>> poolDic = new Dictionary<string, Queue<GameObject>>();
    public List<Pool> poolList = new List<Pool>();

    public void CreatePool(Transform transform)
    {
        foreach (Pool pool in poolList)
        {
            Queue<GameObject> queue = new Queue<GameObject>();

            for (int i = 0; i < pool.amount; i++)
            {
                GameObject obj = Instantiate(pool.prefeb,transform.position,Quaternion.identity);
                obj.transform.SetParent(transform);
                obj.SetActive(false);
                queue.Enqueue(obj);
            }
            poolDic.Add(pool.name, queue);
        }
    }

    public void GetPoolItem(string name,Transform transform)
    {
        if (poolDic.ContainsKey(name))
        {
            GameObject obj = poolDic[name].Dequeue();
            poolDic[name].Enqueue(obj);
            obj.transform.position = transform.position;
            obj.SetActive(true);

        }
        else
        {
            Debug.Log("Can't find Obj");
        }
    }

}
