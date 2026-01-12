using UnityEngine;

public class PlayerSlideState : PlayerMovementState
{
    public PlayerSlideState(PlayerStateContext _context, PlayerStateMachine.PlayerStates _state) : base(_context, _state)
    {
    }

    private float slideTimer;
    private float originalColliderHeight;
    private float originalColliderWidth;
    private Vector3 originalColliderCenter;

    public override void EnterState()
    {
        base.EnterState();
        context.GetInput().Player.Disable();

        UpdateCollider();
        
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
        context.GetPlayerCollider().height = originalColliderHeight;
        context.GetPlayerCollider().radius = originalColliderWidth;
        context.GetPlayerCollider().center = originalColliderCenter;
    }

    private void UpdateCollider()
    {
        originalColliderHeight = context.GetPlayerCollider().height;
        originalColliderWidth = context.GetPlayerCollider().radius;
        originalColliderCenter = context.GetPlayerCollider().center;
        
        context.GetPlayerCollider().height = context.GetSlideColliderHeight();
        context.GetPlayerCollider().radius = context.GetSlideColliderWidth();
        context.GetPlayerCollider().center = context.GetSlideColliderCenter();
    }
}