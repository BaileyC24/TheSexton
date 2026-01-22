using Unity.VisualScripting;
using UnityEngine;

public class PlayerIdleState : PlayerMovementState
{
    public PlayerIdleState(PlayerStateContext _context, PlayerStateMachine.PlayerStates _state) : base(_context, _state)
    {
    }

    public override PlayerStateMachine.PlayerStates GetNextStateKey()
    {
        if (context.CanMove())
            return PlayerStateMachine.PlayerStates.Walk;
        
        return base.GetNextStateKey();
    }
}
