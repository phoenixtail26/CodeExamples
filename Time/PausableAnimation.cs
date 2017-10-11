using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PausableAnimation : MonoBehaviour
{
	Animation _animation;
	public new Animation animation
	{
		get
		{
			return _animation;
		}
	}
	
	Dictionary<string, float> _animationStateSpeeds;
	
	void Awake()
	{
		_animation = GetComponent<Animation>();
		_animationStateSpeeds = new Dictionary<string, float>();
	}
	
	
	// Update is called once per frame
	void Update () 
	{
		foreach(AnimationState animationState in _animation)
		{
			float baseSpeed = 1;
			if(_animationStateSpeeds.ContainsKey(animationState.name))
			{
				baseSpeed = _animationStateSpeeds[animationState.name];
			}
				
			animationState.speed = baseSpeed * GameTime.timeScale;
		}
	}
	
	public void SetSpeed(string key, float speed)
	{
		if(_animationStateSpeeds.ContainsKey(key))
			_animationStateSpeeds[key] = speed;
		else
		{
			bool found = false;
			
			foreach(AnimationState animationState in _animation)
			{
				if(animationState.name == key)
				{
					found = true;
					_animationStateSpeeds.Add(key, speed);
					break;
				}
			}
			
			if(!found)
				DebugUtils.LogWarning("Animation state key not found");
		}
	}
	
	public float GetSpeed(string key)
	{
		if(!_animationStateSpeeds.ContainsKey(key))
		{
			bool found = false;
			
			foreach(AnimationState animationState in _animation)
			{
				if(animationState.name == key)
				{
					found = true;
					_animationStateSpeeds.Add(key, 1f);
					break;
				}
			}
			
			if(!found)
			{
				DebugUtils.LogWarning("Animation state key not found");
				return 1;
			}
		}
		
		return _animationStateSpeeds[key];
	}
}
