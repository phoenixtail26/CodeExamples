using UnityEngine;
using System.Collections;

public class Routine   
{
	static GameObject _routineHolder = null;
	
	MonoBehaviour _behaviour = null;
	Coroutine _coroutine = null;
	
	bool _isRunning = false;
	
	public Coroutine coroutine
	{
		get { return _coroutine; }
	}
	
	public bool isRunning
	{
		get { return _isRunning; }
	}
	
	public Routine( IEnumerator function )
	{
		if ( _routineHolder == null )
		{
			_routineHolder = RoutineHolder.instance.gameObject;
		}
		
		StartRoutine(function);
	}
	
	void StartRoutine( IEnumerator function )
	{
		if ( function != null )
		{
			if ( _behaviour == null )
			{
				_behaviour = _routineHolder.AddComponent<MonoBehaviour>();
			}
			
			_coroutine = _behaviour.StartCoroutine(RunCoroutine(function));
			_isRunning = true;
			
			RoutineHolder.instance.RegisterRoutine(this);
		}
	}
	
	public void Kill()
	{
		if ( _isRunning )
		{
			GameObject.Destroy(_behaviour);
			_behaviour = null;
			_coroutine = null;
			_isRunning = false;
			
			RoutineHolder.instance.DeregisterRoutine(this);
		}
	}
	
	IEnumerator RunCoroutine( IEnumerator function )
	{
		yield return _behaviour.StartCoroutine(function);
		
		Kill();
	}
	
	public static void KillAllRoutines()
	{
		RoutineHolder.instance.KillAllRoutines();
	}
}