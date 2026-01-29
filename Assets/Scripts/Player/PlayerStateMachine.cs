using Sirenix.OdinInspector;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class PlayerStateMachine : StateManager<PlayerStateMachine.PlayerStates>, IDamage, iPickup
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

    public float HPOrig;

    #region Variables

    private PlayerStateContext context;
    private PlayerInput playerInput;


    [BoxGroup("Health Settings")]
    [Title("Base Health")]
    [GUIColor(1f, 0.9f, 0.8f)]
    [Range(10f, 200f), SuffixLabel("hp", Overlay = true)]
    [SerializeField] public float health;

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
    [SerializeField] private Vector3 slideColliderCenter;

    [BoxGroup("References")]
    [GUIColor(1f, 1f, 0.8f)]
    [Required("Rigidbody is required for physics movement.")]
    [SerializeField] private Rigidbody rBody;

    [BoxGroup("References")]
    [GUIColor(1f, 1f, 0.8f)]
    [Required("Collider is required for physics movement.")]
    [SerializeField] private CapsuleCollider collider;

    [BoxGroup("References")]
    [GUIColor(1f, 1f, 0.8f)]
    [Required("Animator is required for physics movement.")]
    [SerializeField] private Animator animator;
    #endregion

    public override void StartMethod()
    {
        spawnPlayer();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerInput = new PlayerInput();
        playerInput.Enable();
        SetupContext();
        SetupState();
    }

    public override void UpdateMethod()
    {
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
            slideColliderHeight,
            slideColliderWidth,
            slideColliderCenter,
            collider,
            playerInput,
            rBody,
            transform,
            Camera.main);
    }


    public void takeDamage(int amount)
    {
        health -= amount;
        updatePlayerUI();
        StartCoroutine(flashDamage());
        if (health <= 0)
        {
            gameManager.instance.youLose();
        }
    }

    public PlayerInput GetInput()
    {
        return playerInput;
    }

    public Animator GetAnimator()
    {
        return animator;
    }

    public void updatePlayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = health / HPOrig;

    }

    IEnumerator flashDamage()
    {
        gameManager.instance.damageFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.damageFlash.SetActive(false);
    }

    public void spawnPlayer()
    {
        HPOrig = health;
        updatePlayerUI();
        transform.position = gameManager.instance.playerSpawnPos.transform.position;
    }


    public void getPowerUps(powerUps heal)
    {

        health = heal.healthCurrent;
    }

    public void GetAllyStats(SurvivorStats survivorStats)
    {
        // Player does not use ally stats
    }
}
