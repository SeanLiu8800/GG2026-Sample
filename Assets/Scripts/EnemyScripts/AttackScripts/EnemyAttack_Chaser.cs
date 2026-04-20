using UnityEngine;
using System.Collections;
public class EnemyAttack_Chaser : EnemyAttackBase
{
    [Header("Attack Variables")]
    [SerializeField] private GameObject attack;

    protected override void Attack()
    {
        if (!enemy.allowAttack) return;
        if (enemy.isBeingPummeled || enemy.isParryStunned) return;
        if (!canAttack || enemy.isAttacking || enemy.target == null) return;

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
        yield break;
    }
}
