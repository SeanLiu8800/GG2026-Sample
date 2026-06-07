using UnityEngine;
using System.Collections;
public class EnemyAttack_DemoBoss : EnemyAttackBase
{
    [Header("Attack Variables")]
    [SerializeField] private GameObject attack;
    [SerializeField] private GameObject meleeAttack;

    protected override void Attack()
    {
        if (!AttackIsPossible()) return;

        enemy.isAttacking = true;
        canAttack = false;

        if (enemy.distanceToTarget < 5.0f) StartCoroutine(MeleeAttack());
        else StartCoroutine(MeleeAttack());
    }

    private IEnumerator MeleeAttack()
    {
        enemy.canMove = false;
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
        enemy.anim.animator.SetBool("IsReadyingAttack", true);
                //enemy.enemyEvents.meleeAttack?.Invoke();
        yield return new WaitForSeconds(0.4f);

        enemy.anim.animator.SetBool("IsReadyingAttack", false);
        enemy.anim.animator.SetBool("IsAttacking", true);
        SpawnAttack(meleeAttack, enemy.target, direction, direction);

        yield return new WaitForSeconds(0.4f);

        enemy.canMove = true;
        enemy.isAttacking = false;
        enemy.anim.animator.SetBool("IsAttacking", false);
        AttackCooldown();
    }
}
