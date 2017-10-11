using UnityEngine;
using System.Collections;

public class CoroutineUtils : AutoSingletonBehaviour<CoroutineUtils>
{
	// Use like this:
	// yield return StartCoroutine(CoroutineUtils.UntilTrue(() => (lives > 3)));
	public static IEnumerator UntilTrue(System.Func<bool> fn)
	{
		while (!fn())
		{
			yield return null;
		}
	}
	// yield return StartCoroutine(CoroutineUtils.WhileAnimating(animation));
	public static IEnumerator WhileAnimating(Animation animation)
	{
		while (animation != null && animation.isPlaying)
		{
			yield return null;
		}
	}
	//Usage :yield return CoroutineUtils.instance.StartCoroutine.EaseWithAction(value => progressBar.Value = value, _oldValue _newValue, 2f, Util.EasingMode.InOut, Util.EasingType.Cubic);
	public static IEnumerator EaseValueWithAction(System.Action<float> result, float startValue, float goalValue, float duration, EasingMode mode = EasingMode.InOut, EasingType type = EasingType.Linear)
	{

		float startTime = Time.realtimeSinceStartup;
		float value = startValue;


		while (startValue < goalValue ? (value < goalValue) : (value > goalValue))
		{
			value = Easing.EaseValue(startTime, Time.realtimeSinceStartup, duration, startValue, goalValue, mode, type);
			result(value);
			yield return null;
		}

		value = goalValue;
		result(value);

	}

	public static Coroutine WhileAnimatingCoroutine(Animation animation)
	{
		return CoroutineUtils.instance.StartCoroutine(WhileAnimating(animation));
	}
	// A wait function using real time
	static IEnumerator WaitForRealSecondsCoroutine(float time)
	{
		while (time > 0.0f)
		{
			float realTime = Time.realtimeSinceStartup;
			yield return null;
			time -= (Time.realtimeSinceStartup - realTime);
		}
	}

	public static Coroutine WaitForRealSeconds(float time)
	{
		return CoroutineUtils.instance.StartCoroutine(WaitForRealSecondsCoroutine(time));
	}
	// A wait function using GameTime instead of Time
	public static IEnumerator WaitForGameSecondsCoroutine(float time)
	{
		time = Mathf.Max(time, 0);
		float timeElapsed = 0;
		while (timeElapsed < time)
		{
			timeElapsed += GameTime.deltaTime;
			
			yield return 0;
		}
	}
	// A wait function that can be skipped by tapping or passing in true to the skipped bool
	public static IEnumerator WaitForSecondsOrTapCoroutine(float time, bool skip, System.Action skippedFunc)
	{
		time = Mathf.Max(time, 0);
		float timeElapsed = 0;
		
		while (timeElapsed < time && !skip)
		{
			timeElapsed += GameTime.deltaTime;
			
			if (Input.GetMouseButtonDown(0))
			{
				skip = true;
				skippedFunc();
			}
			
			yield return 0;
		}
	}

	public static GameObject StartCoroutineOnUniqueGameObject(IEnumerator routine)
	{
		GameObject go = new GameObject(routine.ToString());
		go.transform.parent = instance.transform;
		MonoBehaviour mb = go.AddComponent<MonoBehaviour>();
		mb.StartCoroutine(routine);
		return go;
	}

	public static Coroutine WaitForGameSeconds(float time)
	{
		return CoroutineUtils.instance.StartCoroutine(WaitForGameSecondsCoroutine(time));		
	}

	public static Coroutine WaitForSecondsOrTap(float time, bool skip, System.Action skippedFunc)
	{
		return CoroutineUtils.instance.StartCoroutine(WaitForSecondsOrTapCoroutine(time, skip, skippedFunc));		
	}

	public static Coroutine OnNextFrameCoroutine(System.Action fn)
	{
		return CoroutineUtils.instance.StartCoroutine(OnNextFrame(fn));		
	}
	// yield return StartCoroutine(CoroutineUtils.OnNextFrame(() => { doSomething() } ));
	public static IEnumerator OnNextFrame(System.Action fn)
	{
		yield return null;
		fn();
	}
	// yield return StartCoroutine(CoroutineUtils.AfterGameSeconds(() => { doSomething() } ));
	public static IEnumerator AfterGameSeconds(float time, System.Action fn)
	{
		yield return WaitForGameSeconds(time);
		if (fn != null)
		{
			fn();
		}
	}
	// yield return CoroutineUtils.AfterGameSecondsCoroutine(() => { doSomething() } );
	public static Coroutine AfterGameSecondsCoroutine(float time, System.Action fn)
	{
		return CoroutineUtils.instance.StartCoroutine(AfterGameSeconds(time, fn));		
	}
	// yield return StartCoroutine(CoroutineUtils.AfterSeconds(() => { doSomething() } ));
	public static IEnumerator AfterSeconds(float time, System.Action fn)
	{
		yield return new WaitForSeconds(time);
		if (fn != null)
		{
			fn();
		}
	}
	// yield return CoroutineUtils.AfterSecondsCoroutine(() => { doSomething() } );
	public static Coroutine AfterSecondsCoroutine(float time, System.Action fn)
	{
		return CoroutineUtils.instance.StartCoroutine(AfterSeconds(time, fn));		
	}
	// yield return StartCoroutine(CoroutineUtils.AfterSeconds(() => { doSomething() } ));
	public static IEnumerator AfterRealSeconds(float time, System.Action fn)
	{
		yield return WaitForRealSeconds(time);
		if (fn != null)
		{
			fn();
		}
	}
	// yield return CoroutineUtils.AfterSecondsCoroutine(() => { doSomething() } );
	public static Coroutine AfterRealSecondsCoroutine(float time, System.Action fn)
	{
		return CoroutineUtils.instance.StartCoroutine(AfterRealSeconds(time, fn));		
	}

	public static IEnumerator WhenNotAnimating(Animation anim, System.Action fn)
	{
		while (anim.isPlaying)
		{
			yield return 0;
		}
		fn();
	}

	public static Coroutine WhenNotAnimatingCoroutine(Animation anim, System.Action fn)
	{
		return CoroutineUtils.instance.StartCoroutine(WhenNotAnimating(anim, fn));
	}

	public static IEnumerator WhenTrue(System.Func<bool> fn, System.Action act)
	{
		while (!fn())
		{
			yield return 0;
		}
		act();
	}

	public static Coroutine WhenTrueCoroutine(System.Func<bool> fn, System.Action act)
	{
		return CoroutineUtils.instance.StartCoroutine(WhenTrue(fn, act));
	}
}
