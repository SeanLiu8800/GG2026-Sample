using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class PlayerMovement : PlayerComponent
{
    [SerializeField] private GameObject dashBullet;
    private GameObject currDashBullet = null;

    private InputAction moveAction;
    private InputAction dashAction;

    [Header("Movement Variables")]
    [SerializeField, Range(5.0f, 15.0f)] private float moveSpeed = 10.0f;
    [field: SerializeField, ReadOnly, Range(0.0f, 60.0f)] public float currMoveSpeed { get; private set; } = 0.0f;
    [SerializeField, Range(5.0f, 60.0f)] private float maxMoveSpeed = 20.0f;
    [Range(0.0f, 10.0f)] public float speedRestoreRate = 5.0f;
    [Range(0.0f, 10.0f)] public float speedDecayRate = 5.0f;
    public Vector3 lastMovementDirection { get; private set; } = Vector3.up;
    private Vector3 movementInput;

    [Header("Dash Variables")]
    [SerializeField, ReadOnly] private bool canDash = true;
    [SerializeField, Range(0.0f, 1.5f)] private float dashDuration = 1.0f;
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
    }
    void OnEnable()
    {
        dashAction.started += BufferDash;
        dashAction.canceled += StopDash;

        player.playerEvents.dashStarts += DashStarts;
        player.playerEvents.enhanceAttack += EnhanceAttack;
        player.playerEvents.dashEnds += DashEnds;
        player.playerEvents.perfectDash += PerfectDash;
        player.playerEvents.imperfectDash += ImperfectDash;
        player.playerEvents.dashCooldownEnds += DashCooldownEnds;

        player.playerEvents.pummelStarts += PummelStarts;
        player.playerEvents.pummelEnds += PummelEnds;
        player.playerEvents.pummelReleased += PummelReleased;
        player.playerEvents.pummelEjected += PummelEjected;

        player.playerEvents.lungeStarts += LungeStarts;
        player.playerEvents.lungeEnds += LungeEnds;

        player.playerEvents.knockbackStarts += KnockbackStarts;
        player.playerEvents.knockbackEnds += KnockbackEnds;

        player.playerEvents.attackStarts += AttackStarts;
    }
    void OnDisable()
    {
        dashAction.started -= BufferDash;
        dashAction.canceled -= StopDash;

        player.playerEvents.dashStarts -= DashStarts;
        player.playerEvents.enhanceAttack -= EnhanceAttack;
        player.playerEvents.dashEnds -= DashEnds;
        player.playerEvents.perfectDash -= PerfectDash;
        player.playerEvents.imperfectDash -= ImperfectDash;
        player.playerEvents.dashCooldownEnds -= DashCooldownEnds;
        player.playerEvents.pummelReleased -= PummelReleased;
        player.playerEvents.pummelEjected -= PummelEjected;

        player.playerEvents.pummelStarts -= PummelStarts;
        player.playerEvents.pummelEnds -= PummelEnds;

        player.playerEvents.lungeStarts -= LungeStarts;
        player.playerEvents.lungeEnds -= LungeEnds;

        player.playerEvents.knockbackStarts -= KnockbackStarts;
        player.playerEvents.knockbackEnds -= KnockbackEnds;

        player.playerEvents.attackStarts -= AttackStarts;
    }

    #region ----- Event Functions -----
    void DashStarts()
    {
        dashBuffered = false; // Empty the buffer
        player.spriteRenderer.SetColor(Color.brown.r, Color.brown.g, Color.brown.b, -1.0f);
        willLunge = false;
        canDash = false;
        player.AddState(PlayerState.Dashing);
        player.playerCollider.excludeLayers |= (1 << LayerMask.NameToLayer("Enemy"));
        dashDirection = (movementInput == Vector3.zero) ? lastMovementDirection : movementInput.normalized;
        currDashBullet = Instantiate(dashBullet);
        currDashBullet.GetComponent<BulletScript>().Initialize(gameObject, null, default, dashDirection);
        thisDashEnhancedAttack = false;
        LaunchTowards(dashDirection * 20, dashDuration, 3.0f); // initial Dash Velocity is hard coded to be 20 units
    }
    void EnhanceAttack()
    {
        willLunge = true;
        thisDashEnhancedAttack = true;
    }
    void DashEnds()
    {
        player.RemoveState(PlayerState.Dashing);
        player.playerCollider.excludeLayers &= ~LayerMask.GetMask("Enemy");
        Destroy(currDashBullet);
        StopLaunchTowards();
        player.playerRigidbody.linearVelocity = Vector2.zero;
        // Perfect Dash
        if (Mathf.Abs(0.5f * dashDuration - currLaunchTowardsTime) <= perfectDashLeniency) 
            player.playerEvents.perfectDash?.Invoke();
        // Imperfect Dash
        else player.playerEvents.imperfectDash?.Invoke();
    }
    void PerfectDash()
    {
        currMoveSpeed = Mathf.Clamp(currMoveSpeed * 1.5f, 0.0f, maxMoveSpeed);
        dashCooldownStartTime = Time.time - (dashCooldown * 0.5f);
    }
    void ImperfectDash()
    {
        currMoveSpeed = Mathf.Clamp(currMoveSpeed - 5.0f, moveSpeed * 0.5f, maxMoveSpeed);
        dashCooldownStartTime = Time.time;
    }
    void DashCooldownEnds()
    {
        canDash = true;
        player.spriteRenderer.SetColor(Color.white.r, Color.white.g, Color.white.b, -1.0f); 
    }
    void PummelStarts(Enemy enemy)
    {
        player.playerEvents.dashEnds?.Invoke();
        player.playerEvents.dashCooldownEnds?.Invoke();
        player.playerCollider.enabled = false;
        player.playerRigidbody.linearVelocity = Vector2.zero;
    }
    void PummelEnds(){}
    void PummelReleased(){}
    void PummelEjected(){}
    void LungeStarts()
    {
        player.AddState(PlayerState.Lunging);
        willLunge = false;
        LaunchTowards(lastMovementDirection * lungeInitialSpeed, lungeDuration, 6.0f);
    }
    void LungeEnds()
    {
        player.RemoveState(PlayerState.Lunging);
        StopLaunchTowards();
    }
    void KnockbackStarts(Vector3 initialVelocity, float duration = 0.5f)
    {
        player.playerEvents.dashEnds?.Invoke();
        player.playerEvents.lungeEnds?.Invoke();
        player.AddState(PlayerState.Knockbacked);
        knockbackDuration = duration;
        LaunchTowards(initialVelocity, duration, 6.0f);
    }
    void KnockbackEnds()
    {
        player.RemoveState(PlayerState.Knockbacked);
        StopLaunchTowards();
    }
    void AttackStarts()
    {
        if (player.autoLunge) willLunge = true;
        if (!player.allowLunge) willLunge = false;
        if (player.autoLunge && !player.allowLunge)
            Debug.LogWarning("Player autoLunge is set to True while allowLunge is set to False! allowLunge overrides autoLunge!");
        if (!willLunge) MultiplyMoveSpeed(0.5f);
        else StartAttackLunge();
    }
    #endregion
    
    void Update()
    {
        MoveCharacter();
        UpdateDashBuffer();
        UpdateDashCooldown();
        UpdateKnockback();
    }
    void FixedUpdate()
    {
        CorrectMoveSpeed();
        if (player.isIdle || (player.isAttacking && !player.isLunging)) 
            player.playerRigidbody.linearVelocity = movementInput * currMoveSpeed;
        UpdateDash();
        UpdateAttackLunge();
    }
    private void MoveCharacter()
    {
        movementInput = moveAction.ReadValue<Vector2>();
        if (movementInput != Vector3.zero) lastMovementDirection = movementInput.normalized;
    }
    private float CorrectMoveSpeed()
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
    public void AddMoveSpeed(float addAmount)
    {
        currMoveSpeed = Mathf.Max(currMoveSpeed + addAmount, 0.0f);
    }
    public void MultiplyMoveSpeed(float multiplier)
    {
        currMoveSpeed = Mathf.Max(multiplier * currMoveSpeed, 0.0f);
    }

    [SerializeField, ReadOnly] private bool dashBuffered = false;
    private float bufferStartTime = 0.0f;
    [SerializeField, Range(0.0f, 0.5f)] private float bufferLifespan = 0.1f;
    /// <summary>Fills the Dash Buffer and start it on the first possible frame</summary>
    private void BufferDash(InputAction.CallbackContext context)
    {
        dashBuffered = true;
        bufferStartTime = Time.time;
        UpdateDashBuffer();
    }
    /// <summary>Starts a Dash at the first possible frame if it is buffered, or clears the buffer if it expires</summary>
    private void UpdateDashBuffer()
    {
        if (dashBuffered && Time.time - bufferStartTime <= bufferLifespan)
        {
            if (player.isRestricted) return;
            if (!player.allowDash || !canDash) return;
            if (player.isLunging || player.isPummeling || player.isKnockbacked) return;
            player.playerEvents.dashStarts?.Invoke();
            return;
        }
        dashBuffered = false;
    }
    private void UpdateDash()
    {
        if (!player.isDashing) return;
        // Enhance Attack if player Dashes for the minimum amount of time
        if (!thisDashEnhancedAttack && (player.autoEnhance || currLaunchTowardsTime >= minDashEnhanceAttack)) 
        player.playerEvents.enhanceAttack?.Invoke();
        if (currLaunchTowardsTime >= dashDuration) StopDash(new InputAction.CallbackContext());
    }
    private void StopDash(InputAction.CallbackContext context)
    {
        dashBuffered = false;
        if (!player.isDashing) return;
        player.playerEvents.dashEnds?.Invoke();
    }
    private void UpdateDashCooldown()
    {
        if (player.isDashing || Time.time - dashCooldownStartTime < dashCooldown) return;

        player.playerEvents.dashCooldownEnds?.Invoke();
    }

    [field: Header("Attack Lunge Variables")]
    [field: SerializeField, ReadOnly] public bool willLunge { get; private set; } = false;
    private bool thisDashEnhancedAttack = false;
    [Tooltip("The minimum amount of time to dash to enhance Attack")]
    [SerializeField, Range(0.0f, 0.5f)] private float minDashEnhanceAttack = 0.2f;
    [SerializeField, Range(0.0f, 0.5f)] private float lungeDuration = 0.2f;
    public float lungeInitialSpeed { get { return Mathf.Max(currMoveSpeed * 1.5f, 15.0f); } }
    public float lungeDistance { get { return 0.115f * lungeInitialSpeed; } }
    private void StartAttackLunge()
    {
        player.playerEvents.lungeStarts?.Invoke();
    }
    private void UpdateAttackLunge()
    {
        if (!player.isLunging) return;
        if (currLaunchTowardsTime >= lungeDuration) player.playerEvents.lungeEnds?.Invoke();
    }

    [Header("Knockback Variables")]
    private float currKnockbackTime = -90.0f;
    private float knockbackDuration = 0.5f;
    public void KnockBack(Vector3 initialVelocity, float duration = 0.5f)
    {
        player.playerEvents.knockbackStarts?.Invoke(initialVelocity, duration);
    }
    private void UpdateKnockback()
    {
        if (!player.isKnockbacked) return;
        currKnockbackTime = currLaunchTowardsTime;
        if (currKnockbackTime >= knockbackDuration) player.playerEvents.knockbackEnds?.Invoke();
    }
    private void LaunchTowards(Vector3 startingVelocity, float duration = 0.5f, float lerpCoefficient = 6.0f)
    {
        // Ensure only 1 instances of this coroutine occurs!
        StopLaunchTowards();
        launchTowardsCoroutine = StartCoroutine(LaunchTowardsCoroutine(startingVelocity, duration, lerpCoefficient));
    }
    private void StopLaunchTowards()
    {
        if (launchTowardsCoroutine != null) StopCoroutine(launchTowardsCoroutine);
    }
    private Coroutine launchTowardsCoroutine;
    private Vector3 currLaunchTowardsVelocity = Vector3.zero;
    private float currLaunchTowardsTime = 0.0f;
    private IEnumerator LaunchTowardsCoroutine(Vector3 startingVelocity, float duration = 0.5f, float lerpCoefficient = 3.0f)
    {
        if (duration < 0.0f) duration = 0.0f;
        currLaunchTowardsVelocity = startingVelocity;
        float moveTowardsStartTime = Time.time;
        currLaunchTowardsTime = 0.0f;

        while (currLaunchTowardsTime <= duration)
        {
            player.playerRigidbody.linearVelocity = currLaunchTowardsVelocity;
            currLaunchTowardsVelocity = Vector3.Lerp(currLaunchTowardsVelocity, Vector3.zero, Time.deltaTime * lerpCoefficient);
            currLaunchTowardsTime = Time.time - moveTowardsStartTime;

            yield return null;
        }
        yield break;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.DrawLine(this.transform.position, this.transform.position + lastMovementDirection);
        Gizmos.DrawSphere(this.transform.position + lastMovementDirection, 0.1f);
    }
}
