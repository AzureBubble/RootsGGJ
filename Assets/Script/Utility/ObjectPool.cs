using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private Dictionary<string, Queue<GameObject>> objPool = new Dictionary<string, Queue<GameObject>>();
    private GameObject pool;
    private static ObjectPool instance;

    public static ObjectPool Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ObjectPool();
            }
            return instance;
        }
    }

    public GameObject GetObject(GameObject prefab)
    {
        GameObject _object;
        if (!objPool.ContainsKey(prefab.name) || objPool[prefab.name].Count == 0)
        {
            _object = GameObject.Instantiate(prefab);
            PushObject(_object);
            if (pool == null)
            {
                pool = new GameObject("ObjectPool");
            }
            GameObject childPool = GameObject.Find(prefab.name + "Pool");
            if (!childPool)
            {
                childPool = new GameObject(prefab.name + "Pool");
                childPool.transform.SetParent(pool.transform);
            }
            _object.transform.SetParent(childPool.transform);
        }
        _object = objPool[prefab.name].Dequeue();
        _object.SetActive(true);
        return _object;
    }

    public void PushObject(GameObject prefab)
    {
        string name = prefab.name.Replace("(Clone)", string.Empty);
        if (!objPool.ContainsKey(name))
        {
            objPool.Add(name, new Queue<GameObject>());
        }
        objPool[name].Enqueue(prefab);
        prefab.SetActive(false);
    }
}