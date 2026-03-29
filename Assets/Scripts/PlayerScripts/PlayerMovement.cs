using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : PlayerComponent
{
    [SerializeField] private Collider2D dashCollider;
    [SerializeField] private Rigidbody2D playerRigidbody;

    private InputAction moveAction;
    private InputAction dashAction;

    [Header("Movement Variables")]
    [SerializeField, Range(5.0f, 15.0f)] private float moveSpeed = 5.0f;
    [SerializeField, ReadOnly, Range(0.0f, 60.0f)] private float currMoveSpeed = 0.0f;
    [SerializeField, Range(5.0f, 60.0f)] private float maxMoveSpeed = 30.0f;
    [Range(0.0f, 5.0f)] public float speedRestoreRate = 1.0f;
    [Range(0.0f, 5.0f)] public float speedDecayRate = 1.0f;
    public Vector3 lastMovementDirection { get; private set; } = Vector3.up;
    private Vector3 movementInput;

    [Header("Dash Variables")]
    private bool canDash = true;
    [field : SerializeField, ReadOnly] public bool isDashing { get; private set; } = false;
    private float dashStartTime = -99.9f;
    private Vector3 currDashVelocity;
    [SerializeField, ReadOnly] private float currDashTime = 0.0f;
    [SerializeField, Range(0.0f, 1.5f)] private float dashTime = 2.0f;
    private float dashCooldownStartTime = -99.9f;
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

        player.playerEvents.dashStarts += DashStarts;
        player.playerEvents.enhanceAttack += EnhanceAttack;
        player.playerEvents.dashEnds += DashEnds;
        player.playerEvents.perfectDash += PerfectDash;
        player.playerEvents.imperfectDash += ImperfectDash;
        player.playerEvents.dashCooldownEnds += DashCooldownEnds;

        player.playerEvents.lungeStarts += LungeStarts;
        player.playerEvents.lungeEnds += LungeEnds;

        player.playerEvents.attackStarts += AttackStarts;
    }
    void OnDisable()
    {
        dashAction.started -= StartDash;
        dashAction.canceled -= StopDash;

        player.playerEvents.dashStarts -= DashStarts;
        player.playerEvents.enhanceAttack -= EnhanceAttack;
        player.playerEvents.dashEnds -= DashEnds;
        player.playerEvents.perfectDash -= PerfectDash;
        player.playerEvents.imperfectDash -= ImperfectDash;
        player.playerEvents.dashCooldownEnds -= DashCooldownEnds;

        player.playerEvents.lungeStarts -= LungeStarts;
        player.playerEvents.lungeEnds -= LungeEnds;

        player.playerEvents.attackStarts -= AttackStarts;
    }

    #region ----- Event Functions -----
    void DashStarts()
    {
        AudioManager.Instance.PlaySoundOneShot(AudioManager.Instance.soundEffects.playerDash);
        player.spriteRenderer.SetColor(Color.brown.r, Color.brown.g, Color.brown.b, -1.0f);
        willLunge = false;
        canDash = false;
        isDashing = true;
        player.playerCollider.enabled = false;
        dashCollider.enabled = true;
        dashStartTime = Time.time;
        dashDirection = (movementInput == Vector3.zero) ? lastMovementDirection : movementInput.normalized;
        currDashVelocity = dashDirection * 20;  // Dash is hard coded to be 20 units
        thisDashEnhancedAttack = false;
    }
    void EnhanceAttack()
    {
        willLunge = true;
        thisDashEnhancedAttack = true;
    }
    void DashEnds()
    {
        isDashing = false;
        player.playerCollider.enabled = true;
        dashCollider.enabled = false;
    }
    void PerfectDash()
    {
        currMoveSpeed = Mathf.Clamp(currMoveSpeed * 1.5f, 0.0f, maxMoveSpeed);
        dashCooldownStartTime = Time.time - (dashCooldown * 0.5f);
        AudioManager.Instance.PlaySoundOneShot(AudioManager.Instance.soundEffects.perfectDash);
    }
    void ImperfectDash()
    {
        currMoveSpeed = Mathf.Clamp(currMoveSpeed - 5.0f, moveSpeed * 0.5f, maxMoveSpeed);
        dashCooldownStartTime = Time.time;
        AudioManager.Instance.PlaySoundOneShot(AudioManager.Instance.soundEffects.imperfectDash);
    }
    void DashCooldownEnds()
    {
        canDash = true;
        player.spriteRenderer.SetColor(Color.red.r, Color.red.g, Color.red.b, -1.0f); 
    }
    void LungeStarts()
    {
        isLunging = true;
        willLunge = false;
        lungeStartTime = Time.time;
        currLungeVelocity = lastMovementDirection * Mathf.Max(currMoveSpeed * 1.5f, 15.0f);
    }
    void LungeEnds()
    {
        isLunging = false;
    }
    void AttackStarts()
    {
        if (!willLunge) MultiplyMoveSpeed(0.5f);
        else StartAttackLunge();
    }
    #endregion
    
    void Update()
    {
        MoveCharacter();
        UpdateDashCooldown();
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
    public float GetMoveSpeed() { return currMoveSpeed; }
    public void AddMoveSpeed(float addAmount)
    {
        currMoveSpeed = Mathf.Max(currMoveSpeed + addAmount, 0.0f);
    }
    public void MultiplyMoveSpeed(float multiplier)
    {
        currMoveSpeed = Mathf.Max(multiplier * currMoveSpeed, 0.0f);
    }

    private void StartDash(InputAction.CallbackContext context)
    {
        if (!player.dashIsAvailable) return;
        // Player is Lunging or Dash still on Cooldown
        if (isLunging || Time.time - dashCooldownStartTime < dashCooldown) return;

        player.playerEvents.dashStarts?.Invoke();
    }
    private void Dash()
    {
        if (!isDashing) return;

        currDashTime = Time.time - dashStartTime;
        // Enables Attack Lunging if player Dashes for the minimum amount of time
        if (!thisDashEnhancedAttack && currDashTime >= minDashEnhanceAttack) player.playerEvents.enhanceAttack?.Invoke();
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

        player.playerEvents.dashEnds?.Invoke();
        
        // Perfect Dash
        if (Mathf.Abs(0.5f*dashTime - currDashTime) <= perfectDashLeniency) player.playerEvents.perfectDash?.Invoke();
        // Imperfect Dash
        else player.playerEvents.imperfectDash?.Invoke();
    }
    private void UpdateDashCooldown()
    {
        if (isDashing || Time.time - dashCooldownStartTime < dashCooldown) return;

        player.playerEvents.dashCooldownEnds?.Invoke();
    }

    [field : Header("Attack Lunge Variables")]
    [field: SerializeField, ReadOnly] public bool willLunge { get; private set; } = false;
    [field : SerializeField, ReadOnly] public bool isLunging { get; private set; } = false;
    private bool thisDashEnhancedAttack = false;
    [Tooltip("The minimum amount of time to dash to enhance Attack")]
    [SerializeField, Range(0.0f, 0.5f)] private float minDashEnhanceAttack = 0.2f;
    [SerializeField, Range(0.0f, 0.5f)] private float lungeDuration = 0.2f;
    private float lungeStartTime = 0.0f;
    private float currLungeTime = 0.0f;
    private Vector3 currLungeVelocity;
    public void StartAttackLunge()
    {
        if (!player.lungeIsAvailable) willLunge = false;
        if (!willLunge) return;

        player.playerEvents.lungeStarts?.Invoke();
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

        player.playerEvents.lungeEnds?.Invoke();
    }
    
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.DrawLine(this.transform.position, this.transform.position + lastMovementDirection);
        Gizmos.DrawSphere(this.transform.position + lastMovementDirection, 0.1f);
    }
}
