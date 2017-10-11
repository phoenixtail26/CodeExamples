using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FSMState
{
    public delegate void UpdateDelegate(float timeDelta);

    public string name;
    UpdateDelegate _updateDelegate;

    public FSMState(string stateName, UpdateDelegate del = null)
    {
        name = stateName;
        _updateDelegate = del;
    }

    public void Update(float timeDelta)
    {
        if (_updateDelegate != null)
        {
            _updateDelegate(timeDelta);
        }
    }
}

[System.Serializable]
public class FSM
{
    // TODO: would prefer not to have to compare strings on state change
    public delegate void StateChangeDelegate(string previousState, string nextState);

    Dictionary<string, FSMState> _states = new Dictionary<string, FSMState>();
    FSMState _currentState = null;
    public bool enabled = true;

    public string currentState
    {
        get
        {
            if (_currentState != null)
            {
                return _currentState.name;
            }

            return "";
        }
    }

    public StateChangeDelegate stateChangeDel;

    public FSM()
    {
    }

    public void SetState(string newState)
    {
        if (_states.ContainsKey(newState))
        {
            FSMState nextState = _states[newState];
            if (_currentState != nextState)
            {
                if (stateChangeDel != null)
                {
                    stateChangeDel(_currentState.name, nextState.name);
                }
                _currentState = nextState;
            }
        }
        else
        {
            DebugUtils.LogWarning("FSM: Trying to set state that doesn't exist: " + newState);
        }
    }

    public void AddState(string name, FSMState.UpdateDelegate updateDelegate = null)
    {
        _states.Add(name, new FSMState(name, updateDelegate));
        if (_states.Count == 1)
        {
            _currentState = _states[name];
        }
    }


    public void Update(float timeDelta)
    {
        if (_currentState != null && enabled)
        {
            _currentState.Update(timeDelta);
        }
    }
}

public class FSM<T> where T : struct, System.IConvertible, System.IComparable
{
    public event System.Action<T, T> onStateEnter;
    public event System.Action<T, T> onStateExit;

    public delegate IEnumerator UpdateDelegate();
    public Dictionary<T, UpdateDelegate> onStateUpdate;
    public bool enabled = true;

    int _state;

    public T state
    {
        get
        {
            return (T)(object)(_state);
        }

        set
        {
            if (!state.Equals(value))
            {
                T oldState = state;

                if (onStateExit != null)
                {
                    onStateExit(oldState, value);
                }

                _state = AsInt(value);

                if (onStateEnter != null)
                {
                    onStateEnter(oldState, value);
                }
            }
        }
    }

    int AsInt(T val)
    {
        return (int)(object)(val);
    }

    public FSM(T initialState)
    {
        _state = AsInt(initialState);

        onStateUpdate = new Dictionary<T, UpdateDelegate>();
    }

    /*public void Update(float deltaTime)
    {
        if(onStateUpdate.ContainsKey(_state) && onStateUpdate[_state] != null)
        {
            onStateUpdate[_state](deltaTime);
        }
    }*/

    public IEnumerator UpdateRoutine()
    {
        while (true)
        {
            if (enabled && onStateUpdate.ContainsKey(state) && onStateUpdate[state] != null)
            {
                T lastState = state;
                UpdateDelegate updater = onStateUpdate[state];
                IEnumerator iterator = updater();

                yield return iterator.Current;

                while (lastState.Equals(state) && onStateUpdate[state] == updater)
                {
                    if (!iterator.MoveNext())
                    {
                        break;
                    }
                    else
                    {
                        yield return iterator.Current;
                    }
                }
            }
            else
            {
                yield return null;
            }
        }
    }
}

public class FSMSingletonBehaviour<S, T> : SingletonBehaviour<S>
    where T : struct, System.IConvertible, System.IComparable
    where S : MonoBehaviour
{
    public FSM<T> stateMachine { get; private set; }

    [SerializeField]
    T _initialState;

    public override void Awake()
    {
        base.Awake();

        stateMachine = new FSM<T>(_initialState);
        stateMachine.onStateEnter += OnStateEnter;
        stateMachine.onStateExit += OnStateExit;
    }

    protected virtual void Start()
    {
        StartCoroutine(stateMachine.UpdateRoutine());
    }

    protected virtual void OnStateEnter(T oldState, T newState)
    {
    }

    protected virtual void OnStateExit(T oldState, T newState)
    {
    }
}
