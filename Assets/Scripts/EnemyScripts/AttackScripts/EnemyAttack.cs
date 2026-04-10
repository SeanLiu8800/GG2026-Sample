using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class EnemyAttack : EnemyAttackBase
{
    [Header("Attack Variables")]
    [SerializeField] private GameObject attack;
    [SerializeField] private GameObject meleeAttack;
    [SerializeField] private GameObject meleeSwipe;
    protected void Update()
    {
        Attack();
        if (Keyboard.current.rKey.wasPressedThisFrame) StartCoroutine(Shoot());
        if (Keyboard.current.tKey.wasPressedThisFrame) StartCoroutine(MeleeAttack());
        if (Keyboard.current.gKey.wasPressedThisFrame) StartCoroutine(MeleeSwipes());
    }
    protected override void Attack()
    {
        if (!enemy.allowAttack) return;
        if (enemy.isBeingPummeled) return;
        if (!enemy.canAttack || enemy.target == null) return;
        enemy.canAttack = false;

        if (Random.Range(0, 2) == 1) StartCoroutine(Shoot());
        else StartCoroutine(MeleeAttack());
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

        AttackCooldown();
        yield break;
    }
    private IEnumerator MeleeAttack()
    {
        enemy.canMove = false;
        Vector3 direction = enemy.toTargetDirection;
        AttackWarning();
        yield return new WaitForSeconds(0.2f);
        AttackWarning();

        float dist = enemy.move.DistanceFromImpulse(30.0f);
        AttackZoneManager.Instance.SetSquareAttackZone(
            transform.position + direction * dist,
            direction,
            3.0f,
            2.0f * dist,
            1.0f
        );
        enemy.enemyRigidbody.AddForce(direction * 30.0f, ForceMode2D.Impulse);
        yield return StartCoroutine(TrackDistanceToTarget(2.0f, 0.3f));

        SpawnAttack(meleeAttack, enemy.target, default, direction);
        enemy.enemyRigidbody.AddForce(direction * 10.0f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.15f);

        yield return new WaitForSeconds(0.15f);

        SpawnAttack(meleeAttack, enemy.target, default, direction);
        enemy.enemyRigidbody.AddForce(direction * 10.0f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.2f);

        if (enemy.IsTargetWithinDistance(5.0f))
        {
            StartCoroutine(MeleeAttackFollowup());
        }
        else
        {
            enemy.canMove = true;
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
            enemy.enemyRigidbody.AddForce(direction * 20.0f, ForceMode2D.Impulse);
            SpawnAttack(meleeAttack, enemy.target, default, direction);
            yield return new WaitForSeconds(0.2f);
        }

        enemy.canMove = true;
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
        SetInteractible(true);
        AttackCooldown();
    }
    private Coroutine syncToDirRoulette;
    private IEnumerator SyncToDirRoulette()
    {
        while(true)
        {
            transform.position =
            enemy.target.transform.position - Bullet_OnInterval_DirRouletteEmit.previousDirection * 2.0f;
            yield return null;
        }
    }
}
