using UnityEngine;
using System.Collections;

public class InstantiateOnInstantiate : MonoBehaviour 
{
	[SerializeField]
	GameObject[] _objectsToInstantiate;

	void OnPoolInstantiate()
	{
		for ( int i = 0; i < _objectsToInstantiate.Length; i++ )
		{
			ObjectPool.Instantiate(_objectsToInstantiate[i], transform.position, transform.rotation);
		}
	}
}
