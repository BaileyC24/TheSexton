using UnityEngine;

public class PlayerMovementState : PlayerStateBase
{
    public PlayerMovementState(PlayerStateContext _context, PlayerStateMachine.PlayerStates _state) : base(_context, _state)
    {
    }

    public override void EnterState()
    {
        
    }
    
    public override void UpdateState()
    {
        context.SetMoveValue(context.GetInput().Player.Movement.ReadValue<Vector2>());
        context.CheckForGround();
        context.UpdatePlayerRotation();
    }

    public override void LateUpdateState()
    {
    }

    public override void FixedUpdateState()
    {
        Movement();
    }
    
    public override PlayerStateMachine.PlayerStates GetNextStateKey()
    {
        if (context.IsGrounded())
        {
            if (context.GetInput().Player.Jump.triggered)
                return PlayerStateMachine.PlayerStates.Jump;

            if (context.GetInput().Player.Slide.triggered)
                return PlayerStateMachine.PlayerStates.Slide;
                
        }

        if (!context.IsGrounded())
            return PlayerStateMachine.PlayerStates.Fall;
        
        return StateKey;
    }
    
    public override void ExitState()
    {
    }

}
