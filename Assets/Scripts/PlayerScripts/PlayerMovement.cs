using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class PlayerMovement : PlayerComponent
{
    [SerializeField] private Collider2D dashCollider;
    [SerializeField] private Rigidbody2D playerRigidbody;

    private InputAction moveAction;
    private InputAction dashAction;

    [Header("Movement Variables")]
    [SerializeField, Range(5.0f, 15.0f)] private float moveSpeed = 10.0f;
    [SerializeField, ReadOnly, Range(0.0f, 60.0f)] private float currMoveSpeed = 0.0f;
    [SerializeField, Range(5.0f, 60.0f)] private float maxMoveSpeed = 20.0f;
    [Range(0.0f, 10.0f)] public float speedRestoreRate = 5.0f;
    [Range(0.0f, 10.0f)] public float speedDecayRate = 5.0f;
    public Vector3 lastMovementDirection { get; private set; } = Vector3.up;
    private Vector3 movementInput;

    [Header("Dash Variables")]
    [SerializeField, ReadOnly] private bool canDash = true;
    [field: SerializeField, ReadOnly] public bool isDashing { get; private set; } = false;
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

        player.playerEvents.pummelStarts += PummelStarts;
        player.playerEvents.pummelEnds += PummelEnds;
        player.playerEvents.pummelReleased += PummelReleased;
        player.playerEvents.pummelEjected += PummelEjected;

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
        player.playerEvents.pummelReleased -= PummelReleased;
        player.playerEvents.pummelEjected -= PummelEjected;

        player.playerEvents.pummelStarts -= PummelStarts;
        player.playerEvents.pummelEnds -= PummelEnds;

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
        player.playerCollider.isTrigger = true;
        dashCollider.enabled = true;
        dashDirection = (movementInput == Vector3.zero) ? lastMovementDirection : movementInput.normalized;
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
        isDashing = false;
        player.playerCollider.isTrigger = false;
        dashCollider.enabled = false;
        StopLaunchTowards();
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
    void PummelStarts(Enemy enemy)
    {
        player.playerEvents.dashEnds?.Invoke();
        player.playerCollider.enabled = false;
        playerRigidbody.linearVelocity = Vector2.zero;
    }
    void PummelEnds()
    {
                
    }
    void PummelReleased()
    {
        StartDash(new InputAction.CallbackContext());
        //player.playerEvents.dashStarts?.Invoke();
    }
    void PummelEjected()
    {

    }
    void LungeStarts()
    {
        isLunging = true;
        willLunge = false;
        LaunchTowards(lastMovementDirection * Mathf.Max(currMoveSpeed * 1.5f, 15.0f), lungeDuration, 6.0f);
    }
    void LungeEnds()
    {
        isLunging = false;
        StopLaunchTowards();
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
        UpdateKnockback();
    }
    void FixedUpdate()
    {
        if (!isDashing && !isLunging && !player.pummel.isPummeling && !isKnockbacked) 
            playerRigidbody.linearVelocity = movementInput * CorrectedMoveSpeed();
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
        // Player is Lunging or Dash still on Cooldown or is pummeling
        if (isLunging || !canDash || player.pummel.isPummeling || isKnockbacked) return;

        player.playerEvents.dashStarts?.Invoke();
    }
    private void Dash()
    {
        if (!isDashing) return;
        // Enhance Attack if player Dashes for the minimum amount of time
        if (!thisDashEnhancedAttack && currMoveTowardsTime >= minDashEnhanceAttack) player.playerEvents.enhanceAttack?.Invoke();
        if (currMoveTowardsTime >= dashDuration) StopDash(new InputAction.CallbackContext());
    }
    private void StopDash(InputAction.CallbackContext context)
    {
        if (!isDashing) return;

        player.playerEvents.dashEnds?.Invoke();
        
        // Perfect Dash
        if (Mathf.Abs(0.5f*dashDuration - currMoveTowardsTime) <= perfectDashLeniency) player.playerEvents.perfectDash?.Invoke();
        // Imperfect Dash
        else player.playerEvents.imperfectDash?.Invoke();
    }
    private void UpdateDashCooldown()
    {
        if (isDashing || Time.time - dashCooldownStartTime < dashCooldown) return;

        player.playerEvents.dashCooldownEnds?.Invoke();
    }

    [field: Header("Attack Lunge Variables")]
    [field: SerializeField, ReadOnly] public bool willLunge { get; private set; } = false;
    [field: SerializeField, ReadOnly] public bool isLunging { get; private set; } = false;
    private bool thisDashEnhancedAttack = false;
    [Tooltip("The minimum amount of time to dash to enhance Attack")]
    [SerializeField, Range(0.0f, 0.5f)] private float minDashEnhanceAttack = 0.2f;
    [SerializeField, Range(0.0f, 0.5f)] private float lungeDuration = 0.2f;
    public void StartAttackLunge()
    {
        if (!player.lungeIsAvailable) willLunge = false;
        if (!willLunge) return;

        player.playerEvents.lungeStarts?.Invoke();
    }
    private void AttackLunge()
    {
        if (!isLunging) return;
        if (currMoveTowardsTime >= lungeDuration) StopAttackLunge();
    }
    private void StopAttackLunge()
    {
        if (!isLunging) return;

        player.playerEvents.lungeEnds?.Invoke();
    }

    [Header("Knockback Variables")]
    [field: SerializeField] public bool isKnockbacked { get; private set; } = false;
    private float currKnockbackTime = -90.0f;
    private float knockbackDuration = 0.5f;
    public void KnockBack(Vector3 initialVelocity, float duration = 0.5f)
    {
        Debug.Log("Knockbacked Player!");
        player.playerEvents.dashEnds?.Invoke();
        player.playerEvents.lungeEnds?.Invoke();
        isKnockbacked = true;
        knockbackDuration = duration;
        LaunchTowards(initialVelocity, duration, 6.0f);
    }
    private void UpdateKnockback()
    {
        if (!isKnockbacked) return;
        currKnockbackTime = currMoveTowardsTime;
        if (currKnockbackTime >= knockbackDuration) KnockbackEnds();
    }
    private void KnockbackEnds()
    {
        isKnockbacked = false;
        StopLaunchTowards();
    }
    private void LaunchTowards(Vector3 startingVelocity, float duration = 0.5f, float lerpCoefficient = 6.0f)
    {
        // Ensure only 1 instances of this coroutine occurs!
        StopLaunchTowards();
        moveTowardsCoroutine = StartCoroutine(LaunchTowardsCoroutine(startingVelocity, duration, lerpCoefficient));
    }
    private void StopLaunchTowards()
    {
        if (moveTowardsCoroutine != null) StopCoroutine(moveTowardsCoroutine);
    }
    private Coroutine moveTowardsCoroutine;
    private Vector3 currMoveTowardsVelocity = Vector3.zero;
    private float currMoveTowardsTime = 0.0f;
    private IEnumerator LaunchTowardsCoroutine(Vector3 startingVelocity, float duration = 0.5f, float lerpCoefficient = 3.0f)
    {
        if (duration < 0.0f) duration = 0.0f;
        Debug.Log("Start CO!");
        currMoveTowardsVelocity = startingVelocity;
        float moveTowardsStartTime = Time.time;
        currMoveTowardsTime = 0.0f;

        while (currMoveTowardsTime <= duration)
        {
            playerRigidbody.linearVelocity = currMoveTowardsVelocity;
            currMoveTowardsVelocity = Vector3.Lerp(currMoveTowardsVelocity, Vector3.zero, Time.deltaTime * lerpCoefficient);
            currMoveTowardsTime = Time.time - moveTowardsStartTime;

            yield return null;
        }
        Debug.Log("End CO!");
        yield break;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.DrawLine(this.transform.position, this.transform.position + lastMovementDirection);
        Gizmos.DrawSphere(this.transform.position + lastMovementDirection, 0.1f);
    }
}
