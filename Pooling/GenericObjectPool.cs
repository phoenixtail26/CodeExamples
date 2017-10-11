using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GenericObjectPool<T> where T : UnityEngine.Object
{
	protected static Transform _allPoolsRoot;
	
	protected Transform _poolRoot;
	
	T _prefab = null;
	
	List<T> _availableItems = new List<T>();
	List<T> _usedItems  = new List<T>();
	
	public bool sizeOnDemand = false;
	
	int _initialSize = 0;
	int _poolSize = 0;
	
	protected GenericObjectPool( T prefab, int numPoolItems = 10 )
	{
		if ( _allPoolsRoot == null )
		{
			GameObject go = GameObject.Find("_Pools");
			if ( go == null )
			{
				go = new GameObject("_Pools");
			}
			_allPoolsRoot = go.transform;
		}
		
		_prefab = prefab;
		GameObject poolObj = new GameObject( prefab.name + " Pool" );
		
		_poolRoot = poolObj.transform;
		_poolRoot.parent = _allPoolsRoot;
		
		_initialSize = numPoolItems;
		
		SetPoolSize(numPoolItems);
	}
	
	public void SetPoolSize( int numItems )
	{
		if ( numItems < 0 )
		{
			DebugUtils.LogError("GenericPool: Cannot set pool size to less than zero");
			return;
		}
		
		int currentItemNumber = _availableItems.Count + _usedItems.Count;
		int numNeededItems = numItems - currentItemNumber;
		
		if ( numNeededItems < 0 )
		{
			// Remove unwanted items
			int numToRemove = Mathf.Abs(numNeededItems);
			
			// First remove items that aren't in use
			int availableToRemove = Mathf.Min(numToRemove, _availableItems.Count);
			for ( int i = _availableItems.Count - availableToRemove; i < _availableItems.Count; i++ )
			{
				Object.Destroy(_availableItems[i]);
			}
			_availableItems.RemoveRange(_availableItems.Count - availableToRemove, availableToRemove);
			numToRemove -= availableToRemove;
			
			// Next remove from in use items if needed
			for ( int i = 0; i < numToRemove; i++ )
			{
				Object.Destroy(_usedItems[i]);
			}
			_usedItems.RemoveRange(0, numToRemove);
		}
		else
		{
			// Add new items
			for ( int i = 0; i < numNeededItems; i++ )
			{
				T newItem = CreateItem(currentItemNumber + i);
				_availableItems.Add(newItem);
			}
		}
		
		_poolSize = numItems;
	}
	
	protected virtual T CreateItem( int index )
	{
		T newItem = GameObject.Instantiate(_prefab) as T;
		newItem.name += " " + index;
		return newItem;
	}
	
	public virtual T Instantiate( Vector3 position, Quaternion rotation )
	{
		T item = null;
		if ( _availableItems.Count > 0 )
		{
			// Take from the end of the list
			item = _availableItems[_availableItems.Count-1];
			_availableItems.RemoveAt(_availableItems.Count-1);
		}
		else
		{
			if ( sizeOnDemand )
			{
				SetPoolSize(_poolSize + _initialSize);
				item = _availableItems[_availableItems.Count-1];
				_availableItems.RemoveAt(_availableItems.Count-1);
			}
			else
			{
				// Remove the item that's been around the longest (ie: the first in the list)
				item = _usedItems[0];
				_usedItems.RemoveAt(0);
			}
		}
		
		_usedItems.Add(item);
		return item;
	}
	
	public virtual void Destroy( T item )
	{
		if ( _poolRoot != null )
		{
			_usedItems.Remove(item);
			_availableItems.Add(item);
		}
		else
		{
			Object.Destroy(item);
		}
	}
	
	public void DestroyPool()
	{
		int count = _availableItems.Count;
		for(int i = 0; i < count; i++ )
		{
			Object.Destroy(_availableItems[i]);
		}
		_availableItems.Clear();
		Object.Destroy(_poolRoot.gameObject);
		_poolRoot = null;
	}
	
	public static GenericObjectPool<T> CreatePool( T prefab, int numPoolItems = 10 )
	{
		GenericObjectPool<T> pool = new GenericObjectPool<T>( prefab, numPoolItems );
		return pool;
	}
}