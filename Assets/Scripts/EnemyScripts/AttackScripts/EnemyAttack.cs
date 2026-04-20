using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class EnemyAttack : EnemyAttackBase
{
    [Header("Attack Variables")]
    [SerializeField] private GameObject attack;
    [SerializeField] private GameObject meleeAttack;
    [SerializeField] private GameObject meleeSwipe;
    [SerializeField] private GameObject meleeCircleSweep;
    protected override void Update()
    {
        Attack();
        if (Keyboard.current.rKey.wasPressedThisFrame) StartCoroutine(Shoot());
        if (Keyboard.current.tKey.wasPressedThisFrame) StartCoroutine(MeleeAttack());
        if (Keyboard.current.gKey.wasPressedThisFrame) StartCoroutine(MeleeSwipes());
        if (Keyboard.current.vKey.wasPressedThisFrame) StartCoroutine(MeleeCircleSweep());
    }
    protected override void Attack()
    {
        if (!enemy.allowAttack) return;
        if (enemy.isBeingPummeled || enemy.isParryStunned) return;
        if (!canAttack || enemy.isAttacking || enemy.target == null) return;

        enemy.isAttacking = true;
        canAttack = false;
        switch (Random.Range(0, 4))
        {
            case 0:
                StartCoroutine(Shoot());
                break;
            case 1:
                StartCoroutine(MeleeAttack());
                break;
            case 2:
                StartCoroutine(MeleeSwipes());
                break;
            case 3:
                StartCoroutine(MeleeCircleSweep());
                break;
            default:
                StartCoroutine(Shoot());
                break;
        }
    }
    private IEnumerator Shoot()
    {
        GameObject shootTarget = enemy.target;
        if (shootTarget == null)
        {
            AttackCooldown();
            yield break;
        }
        for (int i = 0; i < 5; i++)
        {
            SpawnAttack(
                attack,
                shootTarget,
                (shootTarget.transform.position - transform.position).normalized,
                shootTarget.transform.position - transform.position
            );
            yield return new WaitForSeconds(0.2f);
        }

        enemy.isAttacking = false;
        AttackCooldown();
    }
    private IEnumerator MeleeAttack()
    {
        enemy.canMove = false;

        AttackWarning();
        yield return new WaitForSeconds(0.2f);
        AttackWarning();

        Vector3 direction = enemy.toTargetDirection;
        float dist = 4.0f;
        AttackZoneManager.Instance.SetSquareAttackZone(
            transform.position + direction * dist,
            direction,
            3.0f,
            2.0f * dist,
            1.0f
        );
        enemy.move.DashToTarget(dist);
        yield return new WaitForSeconds(0.3f);

        SpawnAttack(meleeAttack, enemy.target, default, direction);
        enemy.move.Dash(direction, 1.5f);
        yield return new WaitForSeconds(0.3f);

        SpawnAttack(meleeAttack, enemy.target, default, direction);
        enemy.move.Dash(direction, 1.5f);
        yield return new WaitForSeconds(0.2f);

        if (enemy.IsTargetWithinDistance(5.0f))
        {
            StartCoroutine(MeleeAttackFollowup());
        }
        else
        {
            enemy.canMove = true;
            enemy.isAttacking = false;
            AttackCooldown();
        }
    }
    private IEnumerator MeleeAttackFollowup()
    {
        enemy.canMove = false;

        yield return new WaitForSeconds(0.25f);

        for (int i = 0; i < 2; i ++)
        {
            AttackWarning();
            Vector3 direction = enemy.toTargetDirection;
            AttackZoneManager.Instance.SetCircleAttackZone(
                transform.position + direction * enemy.move.DistanceFromImpulse(20.0f),
                2.0f,
                0.6f
            );
            yield return new WaitForSeconds(0.3f);
            enemy.enemyRigidbody.linearVelocity = Vector2.zero;
            enemy.move.Dash(direction, 2.5f);
            SpawnAttack(meleeAttack, enemy.target, default, direction);
            yield return new WaitForSeconds(0.2f);
        }

        enemy.canMove = true;
        enemy.isAttacking = false;
        AttackCooldown();
    }

    private IEnumerator MeleeSwipes()
    {
        enemy.canMove = false;
        GameObject target = enemy.target;
        if (target == null)
        {
            AttackCooldown();
            yield break;
        }
        yield return new WaitForSeconds(0.3f);
        SetInteractible(false);
        syncToDirRoulette = StartCoroutine(SyncToDirRoulette());
        for (int i = 0; i < 4; i++)
        {
            SpawnAttack(meleeSwipe, target);
            yield return new WaitForSeconds(0.25f);
            AttackWarning();
            yield return new WaitForSeconds(0.55f);
        }
        StopCoroutine(syncToDirRoulette);

        enemy.canMove = true;
        enemy.isAttacking = false;
        SetInteractible(true);
        AttackCooldown();
    }
    private Coroutine syncToDirRoulette;
    private IEnumerator SyncToDirRoulette()
    {
        LayerMask wall;
        float distance = 1.5f + enemy.spriteRenderer.bounds.extents.magnitude;
        if ((wall = LayerMask.GetMask("Wall")) == 0) 
        {
            Debug.LogError("Wall Layer is NOT FOUND");
            yield break;
        }
        while (true)
        {
            Vector3 attackDir = Bullet_OnInterval_DirRouletteEmit.previousDirection;
            Vector3 position = enemy.target.transform.position - attackDir * distance;
            bool enoughSpace = !Physics2D.OverlapBox(position, enemy.spriteRenderer.bounds.extents * 3, 0.0f, wall);
            bool hasLineOfSight = !Physics2D.Raycast(enemy.target.transform.position, -attackDir, distance, wall);
            if (enoughSpace && hasLineOfSight) transform.position = enemy.target.transform.position - attackDir * 2.0f;

            yield return null;
        }
    }

    private IEnumerator MeleeCircleSweep()
    {
        enemy.canMove = false;
        for (int i = 0; i < 4; i ++)
        {
            AttackWarning();
            yield return new WaitForSeconds(0.2f);
            AttackWarning();
            yield return new WaitForSeconds(0.1f);

            Vector3 direction = enemy.toTargetDirection;
            float dist = Mathf.Min(enemy.distanceToTarget - 1.5f, 4.0f);

            AttackZoneManager.Instance.SetCircleAttackZone(
                transform.position + direction * dist,
                4.0f,
                1.1f
            );
            yield return new WaitForSeconds(0.3f);

            enemy.move.Dash(direction, dist);
            SpawnAttack(meleeCircleSweep, enemy.target, default, direction);
            yield return new WaitForSeconds(0.3f);
            SpawnAttack(meleeCircleSweep, enemy.target, default, direction);
            yield return new WaitForSeconds(0.3f);
        }
        
        enemy.canMove = true;
        enemy.isAttacking = false;
        AttackCooldown();
    }
}