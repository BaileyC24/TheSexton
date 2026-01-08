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
    }

    public override void LateUpdateState()
    {
    }

    public override void FixedUpdateState()
    {
    }
    
    public override PlayerStateMachine.PlayerStates GetNextStateKey()
    {
        return StateKey;
    }
    
    public override void ExitState()
    {
    }

}
