using Unity.VisualScripting;
using UnityEngine;

public class PlayerStateMachine : StateManager<PlayerStateMachine.PlayerStates>
{
    public enum PlayerStates
    {
        Idle,
        Walk,
        Sprint,
        Slide
    }
    
    private PlayerStateContext context;
    [SerializeField] private float speed;

    public override void StartMethod()
    {
        SetupState();
        SetupContext();
    }

    public override void UpdateMethod()
    {
    }
    
    private void SetupState()
    {
        States.Add(PlayerStates.Idle, new PlayerIdleState(context, PlayerStates.Idle));
        
        CurrentState = States[PlayerStates.Idle];
    }
    
    private void SetupContext()
    {
        context = new PlayerStateContext(
            speed);
    }
}
