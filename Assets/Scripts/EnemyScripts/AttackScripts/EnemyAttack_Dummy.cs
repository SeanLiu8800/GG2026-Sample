using UnityEngine;
using System.Collections;
public class EnemyAttack_Dummy : EnemyAttackBase
{
    [Header("Attack Variables")]
    [SerializeField] private GameObject bulletAttack;
    [SerializeField] private GameObject meleeAttack;

    protected override void Update()
    {
        Attack();
    }
    protected override void Attack()
    {
        if (!AttackIsPossible()) return;

        enemy.isAttacking = true;
        canAttack = false;

        if (enemy.IsTargetWithinDistance(4.0f)) StartCoroutine(Melee());
        else StartCoroutine(Shoot());
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
                bulletAttack,
                shootTarget,
                (shootTarget.transform.position - transform.position).normalized,
                shootTarget.transform.position - transform.position
            );
            yield return new WaitForSeconds(0.2f);
        }

        enemy.isAttacking = false;
        AttackCooldown();
    }
    private IEnumerator Melee()
    {
        enemy.canMove = false;

        AttackWarning();
        AttackZoneManager.Instance.SetCircleAttackZone(
            transform.position,
            3.0f,
            0.5f
        );

        yield return new WaitForSeconds(0.2f);
        SpawnAttack(meleeAttack, enemy.target, default, enemy.toTargetDirection);
        yield return new WaitForSeconds(0.2f);

        enemy.isAttacking = false;
        AttackCooldown();
    }
}
