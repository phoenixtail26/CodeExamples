using UnityEngine;
using System.Collections;


public class Duration
{
    private float _startTime;
    private float _duration;
    private bool _nonStop;

    public float timeRemaining
    {
        get { return (_startTime + _duration) - _currentTime; }
    }

    public float proportionElapsed
    {
        get
        {
            return Mathf.Clamp01((_currentTime - _startTime) / _duration);
        }
    }

    public bool isElapsed
    {
        get
        {
            return (_currentTime - _startTime) >= _duration;
        }
    }

    float _currentTime
    {
        get { return _nonStop ? NonStopTime.time : Time.time; }
    }

    public float GetEasedValue(float startValue, float endvalue, EasingMode mode, EasingType type)
    {
        return Mathf.Clamp01(Easing.EaseValue(_startTime, _currentTime, _duration, startValue, endvalue, mode, type));
    }

    public float GetEasedProportion(EasingMode mode, EasingType type)
    {
        return Mathf.Clamp01(Easing.EaseValue(_startTime, _currentTime, _duration, 0, 1, mode, type));
    }

    public Duration(float duration, bool nonStop = true)
    {
        _duration = duration;
        _nonStop = nonStop;
        Reset();
    }

    public void Set(float duration)
    {
        _duration = duration;
        Reset();

    }

    public void Reset()
    {
        _startTime = _currentTime;
    }



}
