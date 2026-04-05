using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class EnemyAttack : EnemyComponent
{
    [SerializeField] private Collider2D attackArea;
    [Header("Attack Variables")]
    [SerializeField] private GameObject attack;
    [SerializeField] private GameObject meleeAttack;

    [SerializeField, Range(0.0f, 5.0f)] private float attackCooldown = 1.0f;
    protected virtual void OnEnable()
    {
        enemy.enemyEvents.onEnemyDies += OnEnemyDies;
    }
    protected virtual void OnDisable()
    {
        enemy.enemyEvents.onEnemyDies -= OnEnemyDies;
    }

    #region ----- Event Functions -----
    protected virtual void OnEnemyDies()
    {
        StopAllAttacks();
    }
    #endregion
    
    void Update()
    {
        Attack();
        if (Keyboard.current.tKey.wasPressedThisFrame) StartCoroutine(Shoot());
        if (Keyboard.current.gKey.wasPressedThisFrame) StartCoroutine(MeleeAttack());
    }

    private void Attack()
    {
        if (!enemy.allowAttack) return;
        if (!enemy.canAttack || enemy.target == null) return;
        if (Random.Range(0, 2) == 0) StartCoroutine(Shoot());
        else StartCoroutine(MeleeAttack());
    }
    public void AttackCooldown()
    {
        if (attackCooldownCoroutine != null) StopCoroutine(attackCooldownCoroutine);
        attackCooldownCoroutine = StartCoroutine(AttackCooldownCoroutine());
    }
    private Coroutine attackCooldownCoroutine;
    private IEnumerator AttackCooldownCoroutine()
    {
        enemy.canAttack = false;
        float cooldownStartTime = Time.time;
        while (Time.time - cooldownStartTime <= attackCooldown) yield return null;
        enemy.canAttack = true;
        yield break;
    }
    private void StopAllAttacks()
    {
        StopAllCoroutines();
        AttackCooldown();
    }    
    private IEnumerator Shoot()
    {
        enemy.canAttack = false;
        GameObject shootTarget = enemy.target;
        for (int i = 0; i < 5; i++)
        {
            SpawnAttack(
                attack,
                shootTarget,
                (shootTarget.transform.position - transform.position).normalized * 7.0f,
                shootTarget.transform.position - transform.position
            );
            yield return new WaitForSeconds(0.2f);
        }

        AttackCooldown();
        yield break;
    }
    private IEnumerator MeleeAttack()
    {
        enemy.canAttack = false;
        enemy.canMove = false;
        Vector3 direction = enemy.toTargetDirection;
        yield return new WaitForSeconds(0.2f);
        AttackZoneManager.Instance.SetSquareAttackZone(
            transform.position + direction * 5.0f,
            direction,
            2.5f,
            5.0f,
            1.2f
        );
        enemy.enemyRigidbody.AddForce(direction * 30.0f, ForceMode2D.Impulse);
        float dashStartTime = Time.time;
        while (Time.time - dashStartTime <= 0.3)
        {
            if (enemy.IsTargetWithinDistance(2.0f))
            {
                Debug.Log("Enemy close enough to target");
                enemy.enemyRigidbody.linearVelocity = Vector2.zero;
                break;
            }
            yield return null;
        }
        
        AudioManager.Instance.PlaySoundOneShot(AudioManager.Instance.soundEffects.playerAttack);
        SpawnAttack(meleeAttack, enemy.target, default, direction);
        enemy.enemyRigidbody.AddForce(direction * 10.0f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.3f);
        AudioManager.Instance.PlaySoundOneShot(AudioManager.Instance.soundEffects.playerAttack);
        SpawnAttack(meleeAttack, enemy.target, default, direction);
        enemy.enemyRigidbody.AddForce(direction * 10.0f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f);

        direction = enemy.toTargetDirection;
        AttackZoneManager.Instance.SetCircleAttackZone(
            transform.position + direction * 2.0f,
            2.0f,
            0.6f
        );
        yield return new WaitForSeconds(0.1f);
        enemy.enemyRigidbody.AddForce(direction * 20.0f, ForceMode2D.Impulse);
        AudioManager.Instance.PlaySoundOneShot(AudioManager.Instance.soundEffects.playerAttack);
        SpawnAttack(meleeAttack, enemy.target, default, direction);
        yield return new WaitForSeconds(0.1f);
        enemy.canMove = true;
        AttackCooldown();
    }
    private void SpawnAttack
    (
        GameObject attack, 
        GameObject target, 
        Vector3 initialLinearVelocity = default(Vector3), 
        Vector3 lookDirection = default(Vector3)
    )
    {
        GameObject currAttack = Instantiate(attack, transform.position, transform.rotation);
        if (!currAttack.TryGetComponent<BulletScript>(out BulletScript currBulletScript))
        {
            Debug.LogWarning($"{this.attack.name} DOES NOT have a BulletScript");
        }
        else
        {
            currBulletScript.Initialize(
                this.gameObject,
                target,
                initialLinearVelocity,
                lookDirection
            );
        }
    }
}
