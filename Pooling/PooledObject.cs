using UnityEngine;
using System.Collections;

public class PooledObject : MonoBehaviour
{
    public int initPoolSize = 10;
    public bool broadcastPoolMessagesToChildren = false;
    public GameObjectPool pool;



    public void Destroy()
    {
        if (pool != null)
        {
            pool.Destroy(this.gameObject);
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }
    }

}
