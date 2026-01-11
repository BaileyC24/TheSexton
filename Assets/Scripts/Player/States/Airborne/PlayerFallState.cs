using UnityEngine;

public class PlayerFallState : PlayerMovementState
{
    public PlayerFallState(PlayerStateContext _context, PlayerStateMachine.PlayerStates _state) : base(_context, _state)
    {
    }

    public override PlayerStateMachine.PlayerStates GetNextStateKey()
    {
        if (context.IsGrounded())
            return context.GetInput().Player.Sprint.IsPressed() ? PlayerStateMachine.PlayerStates.Sprint : PlayerStateMachine.PlayerStates.Idle;
        
        return StateKey;
    }
}
