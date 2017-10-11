using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : Singleton<ObjectPool>
{
    Dictionary<Object, GameObjectPool> _poolDictionary = new Dictionary<Object, GameObjectPool>();

    public static GameObject Instantiate(GameObject objectPrefab, Vector3 position, Quaternion rotation)
    {
        if (!InitPool(objectPrefab))
        {
            DebugUtils.LogWarning("ObjectPool: Instantiating new pool at runtime (" + objectPrefab.name + ")", objectPrefab);
        }

        return instance._poolDictionary[objectPrefab].Instantiate(position, rotation);
    }

    public static GameObject Instantiate(GameObject objectPrefab)
    {
        return Instantiate(objectPrefab, Vector3.zero, Quaternion.identity);
    }

    public static GameObject Instantiate(GameObject objectPrefab, Vector3 position)
    {
        return Instantiate(objectPrefab, position, Quaternion.identity);
    }

    public static GameObject Instantiate(GameObject objectPrefab, Transform parent)
    {
        GameObject go = Instantiate(objectPrefab, objectPrefab.transform.position + parent.transform.position, objectPrefab.transform.rotation * parent.transform.rotation);
        if (go != null)
        {
            go.transform.parent = parent;
        }
        return go;
    }


    // Use to initialise all pools at game start
    public static bool InitPool(GameObject objectPrefab)
    {
        if (!instance._poolDictionary.ContainsKey(objectPrefab))
        {
            int startPoolSize = 10;

            PooledObject po = objectPrefab.GetComponent<PooledObject>();
            if (po != null)
            {
                startPoolSize = po.initPoolSize;
            }
            else
            {
                DebugUtils.LogWarning("ObjectPool: Instantiating a pooled object without a PooledObejct component attached (" + objectPrefab.name + ")", objectPrefab);
            }

            GameObjectPool pool = GameObjectPool.CreatePool(objectPrefab, startPoolSize, po.broadcastPoolMessagesToChildren);
            pool.sizeOnDemand = true;
            instance._poolDictionary.Add(objectPrefab, pool);
            return false;
        }

        return true;
    }

    public static void Destroy(GameObject item)
    {
        if (item == null)
        {
            return;
        }

        PooledObject po = item.GetComponent<PooledObject>();
        if (po != null)
        {
            po.Destroy();
        }
        else
        {
            GameObject.Destroy(item);
        }
    }

    public static void DestroyObjectPool()
    {
        StringBuilder sb = new StringBuilder();
        int unusedPoolCount = 0;
        foreach (KeyValuePair<Object, GameObjectPool> kvp in instance._poolDictionary)
        {
            if (!kvp.Value.wasUsed)
            {
                sb.Append(kvp.Key.ToString().Replace(" (UnityEngine.GameObject)", "") + ", ");
                unusedPoolCount++;
            }
        }
        if (unusedPoolCount > 0)
        {
            DebugUtils.Log(unusedPoolCount + " Unused ObjectPools:\n" + sb.ToString().Substring(0, sb.Length - 2));
        }


        instance._poolDictionary.Clear();
    }
}
