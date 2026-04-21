using UnityEngine;
using System.Collections;
public abstract class EnemyAttackBase : EnemyComponent
{
    [Header("Enemy Attack Base Variables")]
    [SerializeField] protected GameObject attackWarning;
    [SerializeField, ReadOnly] protected bool canAttack = true;
    [SerializeField, Range(0.0f, 5.0f)] protected float attackCooldown = 1.0f;
    [SerializeField, ReadOnly] protected bool isOffscreen = false;

    protected virtual void OnEnable()
    {
        enemy.enemyEvents.onEnemyDies += OnEnemyDies;
        enemy.enemyEvents.parryStunStarts += ParryStunStarts;

        enemy.enemyEvents.pummelStarts += PummelStarts;
        enemy.enemyEvents.pummelEnds += PummelEnds;
    }
    protected virtual void OnDisable()
    {
        enemy.enemyEvents.onEnemyDies -= OnEnemyDies;
        enemy.enemyEvents.parryStunStarts -= ParryStunStarts;

        enemy.enemyEvents.pummelStarts -= PummelStarts;
        enemy.enemyEvents.pummelEnds -= PummelEnds;
    }

    #region ----- Event Functions -----
    protected virtual void OnEnemyDies()
    {
        StopAllAttacks();
    }
    protected virtual void ParryStunStarts()
    {
        StopAllAttacks();
    }
    protected virtual void PummelStarts(Player player)
    {
        StopAllAttacks();
    }
    protected virtual void PummelEnds()
    {
        AttackCooldown();
    }
    #endregion

    protected virtual void Update()
    {
        Attack();
    }
    /// <summary>Checks whether an attack is possible this frame</summary>
    /// <returns>Returns True if this enemy CAN Attack, False if it can't</returns>
    protected bool AttackIsPossible()
    {
        if (!enemy.allowAttack) return false;
        if (enemy.isBeingPummeled || enemy.isParryStunned) return false;
        if (!canAttack || enemy.isAttacking || enemy.target == null) return false;

        return true;
    }
    protected abstract void Attack();

    /// <summary>
    /// Check to see whether target is within distance over the specified duration<br/>
    /// Coroutine immediately ends if the target is within distance at any point<br/>
    /// Coroutine will wait until the entire duration otherwise<br/>
    /// </summary>
    /// <param name="distance">The specified distance to compare target to</param>
    /// <param name="duration">The duration to wait</param>
    protected IEnumerator TrackDistanceToTarget(float distance, float duration)
    {
        float dashStartTime = Time.time;
        while (Time.time - dashStartTime <= duration)
        {
            if (enemy.IsTargetWithinDistance(distance))
            {
                enemy.enemyRigidbody.linearVelocity = Vector2.zero;
                yield break;
            }
            yield return null;
        }
    }
    /// <summary>
    /// Sets canAttack to true after this.attackCooldown amount of time
    /// </summary>
    protected void AttackCooldown()
    {
        if (attackCooldownCoroutine != null) StopCoroutine(attackCooldownCoroutine);
        attackCooldownCoroutine = StartCoroutine(AttackCooldownCoroutine());
    }
    private Coroutine attackCooldownCoroutine;
    private IEnumerator AttackCooldownCoroutine()
    {
        canAttack = false;
        float cooldownStartTime = Time.time;
        while (Time.time - cooldownStartTime <= attackCooldown) yield return null;
        canAttack = true;
    }
    /// <summary>
    /// Stops all attacks (with StopAllCoroutines()) then puts this enemy on attack cooldoown
    /// </summary>
    protected void StopAllAttacks()
    {
        StopAllCoroutines();
        enemy.canMove = true;
        enemy.isAttacking = false;
        SetInteractible(true);
        AttackCooldown();
    }
    /// <summary>
    /// Spawn and then Initializes the given Bullet
    /// </summary>
    /// <param name="attack">The bullet to spawn</param>
    /// <param name="target">The target for the bullet</param>
    /// <param name="initialMoveDirection">The Initial Direction of the Bullet</param>
    /// <param name="lookDirection">The direction the Bullet will face</param>
    protected void SpawnAttack
    (
        GameObject attack, 
        GameObject target, 
        Vector3 initialMoveDirection = default(Vector3), 
        Vector3 lookDirection = default(Vector3)
    )
    {
        GameObject currAttack = Instantiate(attack, transform.position, transform.rotation);
        if (!currAttack.TryGetComponent<BulletScript>(out BulletScript currBulletScript))
        {
            Debug.LogWarning($"{currAttack.name} DOES NOT have a BulletScript");
        }
        else
        {
            currBulletScript.Initialize(
                this.gameObject,
                target,
                initialMoveDirection,
                lookDirection
            );
        }
    }
    protected void AttackWarning()
    {
        SpawnAttack(attackWarning, enemy.target, default, default);
    }
    protected void SetInteractible(bool input)
    {
        isOffscreen = !input;
        enemy.enemyCollider.enabled = input;
        enemy.spriteRenderer.enabled = input;
    }
}