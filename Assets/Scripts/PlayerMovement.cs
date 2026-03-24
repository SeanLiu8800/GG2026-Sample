using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : PlayerComponent
{
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private Rigidbody2D playerRigidbody;

    private InputAction moveAction;
    private InputAction dashAction;

    [SerializeField, Range(0.0f, 10.0f)] float moveSpeed = 5.0f;
    [SerializeField, ReadOnly, Range(0.0f, 60.0f)] float currMoveSpeed = 0.0f;
    [SerializeField, ReadOnly, Range(5.0f, 60.0f)] float maxMoveSpeed = 30.0f;
    [Range(0.0f, 5.0f)] public float speedRestoreRate = 1.0f;
    [Range(0.0f, 5.0f)] public float speedDecayRate = 1.0f;
    private Vector3 lastMovementDirection = Vector3.up;
    private Vector3 movementInput;

    private bool isDashing = false;
    private float dashStartTime = 0.0f;
    private Vector3 currDashVelocity;
    [SerializeField, ReadOnly] private float currDashTime = 0.0f;
    [SerializeField, Range(0.0f, 1.5f)] private float dashTime = 2.0f;
    [SerializeField, Range(0.0f, 2.0f)] private float dashCooldown = 0.5f;
    [SerializeField, Range(0.0f, 0.5f)] private float perfectDashLeniency = 0.05f;
    private Vector3 dashDirection;
    protected override void Awake()
    {
        base.Awake();
    
        moveAction = InputSystem.actions.FindAction("Move");
        dashAction = InputSystem.actions.FindAction("Dash");

        currMoveSpeed = moveSpeed;
    }
    void OnEnable()
    {
        dashAction.started += StartDash;
        dashAction.canceled += StopDash;
    }
    void OnDisable()
    {
        dashAction.started -= StartDash;
        dashAction.canceled -= StopDash;
    }
    void Update()
    {
        MoveCharacter();
    }
    void FixedUpdate()
    {
        if (!isDashing) playerRigidbody.linearVelocity = movementInput * CorrectedMoveSpeed();
        if (isDashing) Dash();
    }
    private void MoveCharacter()
    {
        movementInput = moveAction.ReadValue<Vector2>();
        if (movementInput != Vector3.zero) lastMovementDirection = movementInput;
    }
    private float CorrectedMoveSpeed()
    {
        float unitsUnderLimit = moveSpeed - currMoveSpeed;
        float unitsAboveLimit = currMoveSpeed - moveSpeed;
        if (unitsUnderLimit > 0.1f)
        {
            float restoreMultiplier = Mathf.Clamp((unitsUnderLimit / moveSpeed), 1, 99);
            currMoveSpeed += speedRestoreRate * restoreMultiplier * Time.fixedDeltaTime;
        }
        else if (unitsAboveLimit > 0.1f)
        {
            float decayMultiplier = Mathf.Clamp((unitsAboveLimit / moveSpeed), 1, 99);
            currMoveSpeed -= speedDecayRate * decayMultiplier * Time.fixedDeltaTime;
        }
        else
        {
            currMoveSpeed = moveSpeed;
        }
        return currMoveSpeed;
    }
    private void StartDash(InputAction.CallbackContext context)
    {
        // Dash still on Cooldown
        if (Time.time - dashStartTime < dashCooldown)
        {
            AudioManager.Instance.PlaySoundOneShot(AudioManager.Instance.dashFailsSoundEffect);
            return;
        }

        AudioManager.Instance.PlaySoundOneShot(AudioManager.Instance.dashSoundEffect);
        isDashing = true;
        dashStartTime = Time.time;
        dashDirection = (movementInput == Vector3.zero) ? lastMovementDirection : movementInput;
        currDashVelocity = dashDirection * 20;
    }
    private void Dash()
    {
        currDashTime = Time.time - dashStartTime;
        if (currDashTime >= dashTime)
        {
            StopDash(new InputAction.CallbackContext());
            return;
        }
        float coef = currDashTime / dashTime;

        playerRigidbody.linearVelocity = currDashVelocity;
        currDashVelocity = Vector3.Lerp(currDashVelocity, Vector3.zero, Time.fixedDeltaTime * 3.0f);
    }
    private void StopDash(InputAction.CallbackContext context)
    {
        if (!isDashing) return;

        isDashing = false;
        // Perfect Dash
        if (Mathf.Abs(0.5f*dashTime - currDashTime) <= perfectDashLeniency)
        {
            currMoveSpeed = Mathf.Clamp(currMoveSpeed * 1.5f, 0.0f, maxMoveSpeed);
            dashStartTime = Time.time - (dashCooldown / 2.0f);
            AudioManager.Instance.PlaySoundOneShot(AudioManager.Instance.perfectDashSoundEffect);
        }
        // Imperfect Dash
        else
        {
            currMoveSpeed = Mathf.Clamp(currMoveSpeed - 5.0f, moveSpeed * 0.5f, maxMoveSpeed);
            dashStartTime = Time.time;
            AudioManager.Instance.PlaySoundOneShot(AudioManager.Instance.imperfectDashSoundEffect);
        }
    }
}
