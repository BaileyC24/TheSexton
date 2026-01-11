using UnityEngine;

public class PlayerSlideState : PlayerMovementState
{
    public PlayerSlideState(PlayerStateContext _context, PlayerStateMachine.PlayerStates _state) : base(_context, _state)
    {
    }

    private float slideTimer;

    public override void EnterState()
    {
        base.EnterState();
        context.GetInput().Player.Disable();
        
        context.GetRb().linearVelocity += context.GetPlayerTransform().forward * (context.GetSlideTargetDistance() / context.GetSlideFasterDuration());
        slideTimer = Time.deltaTime + context.GetSlideDuration();
    }

    public override void UpdateState()
    {
        base.UpdateState();
        slideTimer -= Time.deltaTime;
    }

    public override PlayerStateMachine.PlayerStates GetNextStateKey()
    {
        if (slideTimer <= 0f)
        {
            return PlayerStateMachine.PlayerStates.Idle;
        }

    return StateKey;
    }

    public override void ExitState()
    {
        base.ExitState();
        context.GetInput().Player.Enable();
    }
}