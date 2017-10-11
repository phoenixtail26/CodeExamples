using UnityEngine;

namespace Framework
{
	[System.Serializable]
    public class GameTimer
    {
        float _timeLength;
        float _time = 0;
		bool _running = false;
		
        public bool hasFinished
        {
            get 
            {
                if ( _time >= _timeLength )
                {
                    return true;
                }
                return false;
            } 
			
			set
			{
				if ( value )
				{
					_time = _timeLength;
				}
				else
				{
					_time = 0;
				}
			}
        }
		
		public bool running
		{
			get { return _running; }
			set { _running = value; }
		}
		
		public float percentageDone
		{
			get 
			{
				return Mathf.Clamp01(_time / _timeLength);
			}
		}

        public GameTimer( float timeLength )
        {
            _timeLength = timeLength;
        }

        // Returns true when the timer has finished
        public bool Update( float deltaTime )
        {
			if ( running )
			{
            	_time += deltaTime;
				if ( hasFinished )
				{
					running = false;
				}
			}
			
            return hasFinished;
        }

        public void Reset()
        {
            _time = 0;
			running = true;
        }

        public void StartTimer()
        {
            running = true;
        }

        public void StopTimer()
        {
            running = false;
        }

        public void SetTimerLength(float time)
        {
            _timeLength = time;
        }
		
		public float GetTimerLength()
		{
			return _timeLength;
		}
		
        public float GetTimeRemaining()
        {
            return Mathf.Max(0, _timeLength - _time);
        }
		
		public float GetTimePassed()
		{
			return _time;
		}
    }
}