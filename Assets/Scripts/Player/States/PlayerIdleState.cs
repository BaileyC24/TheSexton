using Unity.VisualScripting;
using UnityEngine;

public class PlayerIdleState : PlayerMovementState
{
    public PlayerIdleState(PlayerStateContext _context, PlayerStateMachine.PlayerStates _state) : base(_context, _state)
    {
    }

    public override void EnterState()
    {
        Debug.Log("Entering Idle State");
    }
}
