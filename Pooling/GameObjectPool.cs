using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameObjectPool : GenericObjectPool<GameObject>
{
    private GameObjectPool(GameObject prefab, int numPoolItems = 10, bool broadcastToChildren = false)
        : base(prefab, numPoolItems)
    {
        _broadcastToChildren = broadcastToChildren;
    }
    bool _wasUsed = false;
    private bool _broadcastToChildren = false;
    public bool wasUsed
    {
        get { return _wasUsed; }
    }

    protected override GameObject CreateItem(int index)
    {
        GameObject go = base.CreateItem(index);
        PooledObject po = go.GetComponent<PooledObject>();
        if (po != null)
        {
            po.pool = this;
        }
        go.transform.parent = _poolRoot;
        go.SetActive(false);
        return go;
    }

    public override GameObject Instantiate(Vector3 position, Quaternion rotation)
    {
        GameObject go = base.Instantiate(position, rotation);
        if (go != null)
        {
            go.transform.position = position;
            go.transform.rotation = rotation;
            go.SetActive(true);

            //NOTE: Messages are not sent to inactive objects
            if (_broadcastToChildren)
            {
                go.BroadcastMessage("OnPoolInstantiate", SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                go.SendMessage("OnPoolInstantiate", SendMessageOptions.DontRequireReceiver);
            }

            go.transform.parent = null;
        }

        _wasUsed = true;
        return go;
    }

    public override void Destroy(GameObject item)
    {

        //NOTE: Messages are not sent to inactive objects
        if (_broadcastToChildren)
        {
            item.BroadcastMessage("OnPoolDestroy", SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            item.SendMessage("OnPoolDestroy", SendMessageOptions.DontRequireReceiver);
        }

        item.SetActive(false);

        if (_poolRoot != null)
        {
            item.transform.parent = _poolRoot;
        }

        base.Destroy(item);
    }

    public static GameObjectPool CreatePool(GameObject prefab, int numPoolItems = 10, bool broadcastToChildren = false)
    {
        GameObjectPool pool = new GameObjectPool(prefab, numPoolItems, broadcastToChildren);
        return pool;
    }
}

