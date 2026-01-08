using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateManager<EnumState> : MonoBehaviour where EnumState : Enum
{
    protected Dictionary<EnumState, StateBase<EnumState>> States = new Dictionary<EnumState, StateBase<EnumState>>();
    public StateBase<EnumState> CurrentState { get; protected set; }
    private bool changingState;

    public abstract void StartMethod();
    public abstract void UpdateMethod();
    
    private void Start()
    {
        StartMethod();
        CurrentState?.EnterState();
    }

    private void Update()
    {
        UpdateMethod();
        if (changingState || CurrentState == null)
            return;
        
        EnumState nextStateKey = CurrentState.GetNextStateKey();

        if (nextStateKey.Equals(CurrentState.StateKey))
        {
            CurrentState.UpdateState();
        } else
        {
            ChangeState(nextStateKey);
        }
    }
    
    void LateUpdate()
    {
        if (changingState || CurrentState == null)
            return;
        
        CurrentState.LateUpdateState();
    }

    void FixedUpdate()
    {
        if (!changingState || CurrentState == null)
            return;
        
        CurrentState.FixedUpdateState();
    }

    private void ChangeState(EnumState _newState)
    {
        if (CurrentState != null && _newState.Equals(CurrentState.StateKey))
            return;
        
        changingState = true;
        
        CurrentState?.ExitState();
        CurrentState = States[_newState];
        CurrentState?.EnterState();
        
        changingState = false;
    }
}
