using UnityEngine;

public class PlayerJumpState : PlayerMovementState
{
    public PlayerJumpState(PlayerStateContext _context, PlayerStateMachine.PlayerStates _state) : base(_context, _state)
    {
    }

    public override void EnterState()
    {
        Vector3 linearVelocity = context.GetRb().linearVelocity;
        linearVelocity.y += 5f;
        context.GetRb().linearVelocity = linearVelocity;
    }

    public override PlayerStateMachine.PlayerStates GetNextStateKey()
    {
        if (context.GetRb().linearVelocity.y <= -0.5)
            return PlayerStateMachine.PlayerStates.Fall;
        
        return StateKey;
    }
}