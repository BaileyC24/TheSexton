using System;
using UnityEngine;

public abstract class StateBase<EnumState> where EnumState : Enum
{
    protected StateBase(EnumState _stateKey)
    {
        StateKey = _stateKey;
    }

    public EnumState StateKey { get; private set; }
    public abstract void EnterState();
    public abstract void ExitState();
    public abstract EnumState GetNextStateKey();
    public abstract void UpdateState();
    public abstract void LateUpdateState();
    public abstract void FixedUpdateState();
}
