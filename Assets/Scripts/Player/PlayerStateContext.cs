using UnityEngine;

public class PlayerStateContext
{
    private float speed;
    private float sprintSpeed;
    private float slideTargetDistance;
    private float slideFasterDuration; 
    private float slideDuration;
    private float slideColliderHeight;
    private float slideColliderWidth;
    private Vector3 slideColliderCenter;
    private PlayerInput input;
    private Vector2 moveValue;
    private Rigidbody rBody;
    private Camera camera;
    private Transform playerTransform;
    private CapsuleCollider playerCollider;
    private bool isGrounded;

    public PlayerStateContext(
        float _speed,
        float _sprintSpeed,
        float _slideTargetDistance,
        float _slideFasterDuration,
        float _slideDuration,
        float _slideColliderHeight,
        float _slideColliderWidth,
        Vector3 _slideColliderCenter,
        CapsuleCollider _playerCollider,
        PlayerInput _input,
        Rigidbody _rBody,
        Transform _playerTransform,
        Camera _camera)
    {
        speed = _speed;
        sprintSpeed = _sprintSpeed;
        slideTargetDistance = _slideTargetDistance;
        slideFasterDuration = _slideFasterDuration;
        slideDuration = _slideDuration;
        slideColliderHeight = _slideColliderHeight;
        slideColliderWidth = _slideColliderWidth;
        slideColliderCenter = _slideColliderCenter;
        playerCollider = _playerCollider;
        input = _input;
        rBody = _rBody;
        playerTransform = _playerTransform;
        camera = _camera;
    }

    #region Getters
    public float GetSpeed() => speed;
    public float GetSprintSpeed() => sprintSpeed;
    public float GetSlideTargetDistance() => slideTargetDistance;
    public float GetSlideFasterDuration() => slideFasterDuration;
    public float GetSlideDuration() => slideDuration;
    public float GetSlideColliderHeight() => slideColliderHeight;
    public CapsuleCollider GetPlayerCollider() => playerCollider;
    public float GetSlideColliderWidth() => slideColliderWidth;
    public Vector3 GetSlideColliderCenter() => slideColliderCenter;
    public PlayerInput GetInput() => input;
    public Vector2 GetMoveValue() => moveValue;
    public bool CanMove() => GetMoveValue().magnitude > 0.01f;
    public Rigidbody GetRb() => rBody;
    public Transform GetPlayerTransform() => playerTransform;
    public Camera GetCamera() => camera;
    public bool IsGrounded() => isGrounded;
    #endregion
    
    #region Setters
    public void SetMoveValue(Vector2 _value) => moveValue = _value;
    public void SetSpeed(float _value) => speed = _value;
    #endregion

    public void CheckForGround()
    {
        isGrounded = Physics.Raycast(GetPlayerTransform().position, -GetPlayerTransform().up,
            1.1f, ~LayerMask.GetMask("Player"));
        Debug.DrawRay(GetPlayerTransform().position, -GetPlayerTransform().up * 1.1f, Color.red);
    }

    public void UpdatePlayerRotation()
    {
        Vector3 cameraForward = GetCamera().transform.forward;
        Vector3 playerLookDirection = new Vector3(cameraForward.x, 0, cameraForward.z);
        
        if (playerLookDirection != Vector3.zero)
        {
            GetPlayerTransform().rotation = Quaternion.LookRotation(playerLookDirection);
        }
    }


}