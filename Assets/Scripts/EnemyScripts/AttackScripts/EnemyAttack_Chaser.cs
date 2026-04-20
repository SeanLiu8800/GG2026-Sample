using UnityEngine;
using System.Collections;
public class EnemyAttack_Chaser : EnemyAttackBase
{
    [Header("Attack Variables")]
    [SerializeField, Range(1.0f, 5.0f)] private float attackRange = 5.0f;
    [SerializeField] private GameObject attack;

    protected override void Attack()
    {
        if (!enemy.allowAttack) return;
        if (enemy.isBeingPummeled || enemy.isParryStunned) return;
        if (!canAttack || enemy.isAttacking || enemy.target == null) return;

        if (!enemy.IsTargetWithinDistance(attackRange)) return;
        enemy.isAttacking = true;
        canAttack = false;
        switch (Random.Range(0, 1))
        {
            case 0:
                StartCoroutine(LungeAndAttack());
                break;
            default:
                StartCoroutine(LungeAndAttack());
                break;
        }
    }

    private IEnumerator LungeAndAttack()
    {
        enemy.canMove = false;

        Vector3 direction = enemy.toTargetDirection;
        float dist = 4.0f;
        AttackZoneManager.Instance.SetSquareAttackZone(
            transform.position + direction * dist,
            direction,
            3.0f,
            2.0f * dist,
            1.0f
        );
        AttackWarning();
        yield return new WaitForSeconds(0.2f);

        
        enemy.move.Dash(direction, dist);
        SpawnAttack(attack, enemy.target, default, direction);

        yield return new WaitForSeconds(0.2f);

        enemy.canMove = true;
        enemy.isAttacking = false;
        AttackCooldown();
    }
}
