using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenericPool<T> : GenericObjectPool<T> where T: MonoBehaviour
{
	private GenericPool( T prefab, int numPoolItems = 10 ) : base ( prefab, numPoolItems ) {}
	
	protected override T CreateItem (int index)
	{
		T item = base.CreateItem (index);
		item.transform.parent = _poolRoot;
		item.gameObject.SetActive(false);
		return item;
	}
	
	public override T Instantiate( Vector3 position, Quaternion rotation )
	{
		T item = base.Instantiate(position,rotation);
		item.transform.position = position;
		item.transform.rotation = rotation;
		item.gameObject.SetActive(true);
		item.SendMessage("OnPoolInstantiate", SendMessageOptions.DontRequireReceiver);
		item.transform.parent = null;
		return item;
	}
	
	public override void Destroy( T item )
	{
		item.SendMessage("OnPoolDestroy", SendMessageOptions.DontRequireReceiver);
		item.gameObject.SetActive(false);
		
		if ( _poolRoot != null )
		{
			item.transform.parent = _poolRoot;
		}
		
		base.Destroy(item);
	}
	
	public static new GenericPool<T> CreatePool( T prefab, int numPoolItems = 10 )
	{
		GenericPool<T> pool = new GenericPool<T>( prefab, numPoolItems );
		return pool;
	}
}
