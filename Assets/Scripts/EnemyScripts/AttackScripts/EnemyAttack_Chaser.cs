using UnityEngine;
using System.Collections;
public class EnemyAttack_Chaser : EnemyAttackBase
{
    [Header("Attack Variables")]
    [SerializeField, Range(3.0f, 8.0f)] private float attackRange = 5.0f;
    [SerializeField] private GameObject attack;

    protected override void Attack()
    {
        if (!AttackIsPossible()) return;

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
        yield return new WaitForSeconds(0.4f);

        
        enemy.move.Dash(direction, dist);
        SpawnAttack(attack, enemy.target, default, direction);

        yield return new WaitForSeconds(0.2f);

        enemy.canMove = true;
        enemy.isAttacking = false;
        AttackCooldown();
    }
}
