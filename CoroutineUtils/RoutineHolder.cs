using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoutineHolder : AutoSingletonBehaviour<RoutineHolder>
{
	List<Routine> _activeRoutines = new List<Routine>();
	
	public override void Awake()
	{
		base.Awake();
		
		if(this == instance)
		{
			DontDestroyOnLoad(this.gameObject);
		}
	}
	
	public void KillAllRoutines()
	{
		List<Routine> currentRoutines = new List<Routine>();
		currentRoutines.AddRange(_activeRoutines);
		foreach ( Routine r in currentRoutines )
		{
			r.Kill();
		}
	}
	
	public void RegisterRoutine( Routine r )
	{
		if ( !_activeRoutines.Contains(r) )
		{
			_activeRoutines.Add(r);
		}
	}
	
	public void DeregisterRoutine( Routine r )
	{
		_activeRoutines.Remove(r);
	}
	
}
