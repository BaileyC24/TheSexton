using UnityEngine;

public class PlayerSprintState : PlayerMovementState
{
    public PlayerSprintState(PlayerStateContext _context, PlayerStateMachine.PlayerStates _state) : base(_context, _state)
    {
    }
    private float speedOrig;
    
    public override void EnterState()
    {
        base.EnterState();
        speedOrig = context.GetSpeed();
        context.SetSpeed(context.GetSprintSpeed());
    }

    public override PlayerStateMachine.PlayerStates GetNextStateKey()
    {
        if (!context.GetInput().Player.Sprint.IsPressed())
        {
            return PlayerStateMachine.PlayerStates.Idle;
        }
        
        return base.GetNextStateKey();
    }

    public override void ExitState()
    {
        base.ExitState();
        context.SetSpeed(speedOrig);
    }
}
