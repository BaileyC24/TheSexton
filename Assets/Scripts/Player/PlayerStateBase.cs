using UnityEngine;

public abstract class PlayerStateBase : StateBase<PlayerStateMachine.PlayerStates>
{
    protected readonly PlayerStateContext context;
    protected PlayerStateBase(PlayerStateContext _context, PlayerStateMachine.PlayerStates _state) :
        base(_state)
    {
        context = _context;
    }
}