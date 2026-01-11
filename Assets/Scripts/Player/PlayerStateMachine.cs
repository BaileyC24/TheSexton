using Sirenix.OdinInspector;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStateMachine : StateManager<PlayerStateMachine.PlayerStates>
{
    public enum PlayerStates
    {
        Idle,
        Walk,
        Jump,
        Fall,
        Sprint,
        Slide
    }

    #region Variables
    
    private PlayerStateContext context;
    private PlayerInput playerInput;
    
    [BoxGroup("Movement Settings")]
    [Title("Base Speeds")]
    [GUIColor(0.8f, 1f, 0.8f)]
    [Range(1f, 20f), SuffixLabel("m/s", Overlay = true)]
    [SerializeField] private float speed;

    [BoxGroup("Movement Settings")]
    [GUIColor(0.8f, 1f, 0.8f)]
    [Range(1f, 30f), SuffixLabel("m/s", Overlay = true)]
    [SerializeField] private float sprintSpeed;
    
    [BoxGroup("Slide Settings")]
    [Title("Duration & Distance")]
    [GUIColor(1f, 0.9f, 0.8f)]
    [Tooltip("How far the player should slide.")]
    [Range(1f, 20f), SuffixLabel("meters")]
    [SerializeField] private float slideTargetDistance;

    [BoxGroup("Slide Settings")]
    [GUIColor(1f, 0.9f, 0.8f)]
    [Tooltip("How quickly the slide finishes when using distance calc.")]
    [Range(0.1f, 2f), SuffixLabel("sec", Overlay = true)]
    [SerializeField] private float slideFasterDuration;

    [BoxGroup("Slide Settings")]
    [GUIColor(1f, 0.9f, 0.8f)]
    [Tooltip("Base duration if not using distance calculation.")]
    [Range(0.1f, 3f), SuffixLabel("sec", Overlay = true)]
    [SerializeField] private float slideDuration;
    
    [FoldoutGroup("Advanced Physics")]
    [Title("Slide Collider Adjustments")]
    [GUIColor(0.8f, 0.9f, 1f)]
    [Range(0.1f, 2f)]
    [SerializeField] private float slideColliderHeight;

    [FoldoutGroup("Advanced Physics")]
    [GUIColor(0.8f, 0.9f, 1f)]
    [Range(0.1f, 2f)]
    [SerializeField] private float slideColliderWidth;

    [FoldoutGroup("Advanced Physics")]
    [GUIColor(0.8f, 0.9f, 1f)]
    [Range(-1f, 1f)]
    [SerializeField] private float slideColliderCenter;
    
    [BoxGroup("References")]
    [GUIColor(1f, 1f, 0.8f)]
    [Required("Rigidbody is required for physics movement.")]
    [SerializeField] private Rigidbody rBody;

    [BoxGroup("References")]
    [GUIColor(1f, 1f, 0.8f)]
    [SceneObjectsOnly]
    [SerializeField] private TextMeshProUGUI stateText;
    #endregion

    public override void StartMethod()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerInput = new PlayerInput();
        playerInput.Enable();
        SetupContext();
        SetupState();
    }

    public override void UpdateMethod()
    {
        stateText.text = "State: " + CurrentState.StateKey;
    }
    
    private void SetupState()
    {
        States.Add(PlayerStates.Idle, new PlayerIdleState(context, PlayerStates.Idle));
        States.Add(PlayerStates.Fall, new PlayerFallState(context, PlayerStates.Fall));
        States.Add(PlayerStates.Jump, new PlayerJumpState(context, PlayerStates.Jump));
        States.Add(PlayerStates.Sprint, new PlayerSprintState(context, PlayerStates.Sprint));
        States.Add(PlayerStates.Slide, new PlayerSlideState(context, PlayerStates.Slide));
        States.Add(PlayerStates.Walk, new PlayerWalkState(context, PlayerStates.Walk));
        
        CurrentState = States[PlayerStates.Idle];
    }
    
    private void SetupContext()
    {
        context = new PlayerStateContext(
            speed,
            sprintSpeed,
            slideTargetDistance,
            slideFasterDuration,
            slideDuration,
            playerInput,
            rBody,
            transform,
            Camera.main);
    }
}
