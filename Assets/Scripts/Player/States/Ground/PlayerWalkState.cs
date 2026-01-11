using UnityEngine;

public class PlayerWalkState : PlayerMovementState
{
    public PlayerWalkState(PlayerStateContext _context, PlayerStateMachine.PlayerStates _state) : base(_context, _state)
    {
    }

    public override PlayerStateMachine.PlayerStates GetNextStateKey()
    {
        if (!context.CanMove())
        {
            return PlayerStateMachine.PlayerStates.Idle;
        }
        
        if (context.GetInput().Player.Sprint.IsPressed())
        {
            return PlayerStateMachine.PlayerStates.Sprint;
        }

        return base.GetNextStateKey();
    }
}