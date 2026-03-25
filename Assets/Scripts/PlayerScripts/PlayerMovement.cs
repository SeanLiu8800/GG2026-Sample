using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : PlayerComponent
{
    [SerializeField] private Collider2D dashCollider;
    [SerializeField] private Rigidbody2D playerRigidbody;

    private InputAction moveAction;
    private InputAction dashAction;

    [Header("Movement Variables")]
    [SerializeField, Range(5.0f, 15.0f)] float moveSpeed = 5.0f;
    [SerializeField, ReadOnly, Range(0.0f, 60.0f)] float currMoveSpeed = 0.0f;
    [SerializeField, Range(5.0f, 60.0f)] float maxMoveSpeed = 30.0f;
    [Range(0.0f, 5.0f)] public float speedRestoreRate = 1.0f;
    [Range(0.0f, 5.0f)] public float speedDecayRate = 1.0f;
    public Vector3 lastMovementDirection { get; private set; } = Vector3.up;
    private Vector3 movementInput;

    [field : Header("Dash Variables")]
    [field : SerializeField, ReadOnly] public bool isDashing { get; private set; } = false;
    private float dashStartTime = 0.0f;
    private Vector3 currDashVelocity;
    [SerializeField, ReadOnly] private float currDashTime = 0.0f;
    [SerializeField, Range(0.0f, 1.5f)] private float dashTime = 2.0f;
    [SerializeField, Range(0.0f, 2.0f)] private float dashCooldown = 0.5f;
    [SerializeField, Range(0.0f, 0.5f)] private float perfectDashLeniency = 0.05f;
    public Vector3 dashDirection { get; private set; }
    protected override void Awake()
    {
        base.Awake();
    
        moveAction = InputSystem.actions.FindAction("Move");
        dashAction = InputSystem.actions.FindAction("Dash");

        currMoveSpeed = moveSpeed;
        player.playerCollider.enabled = true;
        dashCollider.enabled = false;
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
        Dash();
        AttackLunge();
    }
    private void MoveCharacter()
    {
        movementInput = moveAction.ReadValue<Vector2>();
        if (movementInput != Vector3.zero) lastMovementDirection = movementInput.normalized;
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
        if (isLunging || Time.time - dashStartTime < dashCooldown)
        {
            AudioManager.Instance.PlaySoundOneShot(AudioManager.Instance.dashFailsSoundEffect);
            return;
        }

        AudioManager.Instance.PlaySoundOneShot(AudioManager.Instance.dashSoundEffect);
        isDashing = true;
        player.playerCollider.enabled = false;
        dashCollider.enabled = true;
        dashStartTime = Time.time;
        dashDirection = (movementInput == Vector3.zero) ? lastMovementDirection : movementInput.normalized;
        currDashVelocity = dashDirection * 20;  // Dash is hard coded to be 20 units

        player.attack.ResetDamage();
    }
    private void Dash()
    {
        if (!isDashing) return;

        currDashTime = Time.time - dashStartTime;
        // Enables Attack Lunging if player Dashes for the minimum amount of time
        if (currDashTime >= minDashEnableLunge) canLunge = true;
        if (currDashTime >= dashTime)
        {
            StopDash(new InputAction.CallbackContext());
            return;
        }

        playerRigidbody.linearVelocity = currDashVelocity;
        currDashVelocity = Vector3.Lerp(currDashVelocity, Vector3.zero, Time.fixedDeltaTime * 3.0f);
    }
    private void StopDash(InputAction.CallbackContext context)
    {
        if (!isDashing) return;

        isDashing = false;
        player.playerCollider.enabled = true;
        dashCollider.enabled = false;
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

    [field : Header("Attack Lunge Variables")]
    [field: SerializeField, ReadOnly] public bool canLunge { get; private set; } = false;
    [field : SerializeField, ReadOnly] public bool isLunging { get; private set; } = false;
    [Tooltip("The minimum amount of time to dash to enable Attack Lunging")]
    [SerializeField, Range(0.0f, 0.5f)] private float minDashEnableLunge = 0.2f;
    [SerializeField, Range(0.0f, 0.5f)] private float lungeDuration = 0.2f;
    private float lungeStartTime = 0.0f;
    private float currLungeTime = 0.0f;
    private Vector3 currLungeVelocity;
    public void StartAttackLunge()
    {
        if (!canLunge) return;

        isLunging = true;
        canLunge = false;
        lungeStartTime = Time.time;
        currLungeVelocity = lastMovementDirection * Mathf.Max(currMoveSpeed * 1.5f, 15.0f);
    }
    private void AttackLunge()
    {
        if (!isLunging) return;

        currLungeTime = Time.time - lungeStartTime;
        if (currLungeTime >= lungeDuration)
        {
            StopAttackLunge();
            return;
        }

        playerRigidbody.linearVelocity = currLungeVelocity;
        currLungeVelocity = Vector3.Lerp(currLungeVelocity, Vector3.zero, Time.fixedDeltaTime * 6.0f);
    }
    private void StopAttackLunge()
    {
        if (!isLunging) return;

        isLunging = false;
    }
    
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.DrawLine(this.transform.position, this.transform.position + lastMovementDirection);
        Gizmos.DrawSphere(this.transform.position + lastMovementDirection, 0.1f);
    }
}
