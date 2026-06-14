using UnityEngine;
using System.Collections;
public class EnemyAttack_DemoBoss : EnemyAttackBase
{
    [Header("Attack Variables")]
    [SerializeField] private GameObject attack;
    [SerializeField] private GameObject radialEmitter;
    [SerializeField] private GameObject localSlam;
    [SerializeField] private GameObject meleeAttack;

    private int[] attackHistory = new int[2] { -1, -1 };
    private int attackHistoryIndex = 0;
    protected override void Attack()
    {
        if (!AttackIsPossible()) return;

        enemy.isAttacking = true;
        canAttack = false;

        if (enemy.distanceToTarget < 5.0f) StartCoroutine(MeleeAttack());
        else
        {
            int attackNumber;
            while (true)
            {
                attackNumber = Random.Range(0, 3);
                if (attackNumber != attackHistory[0] || attackNumber != attackHistory[0]) break;
            }
            switch (attackNumber)
            {
                case 0:
                    StartCoroutine(MeleeAttack());
                    break;
                case 1:
                    StartCoroutine(RadialEmit());
                    break;
                default:
                    StartCoroutine(Shoot());
                    break;
            }
            attackHistory[attackHistoryIndex] = attackNumber;
            attackHistoryIndex = (attackHistoryIndex + 1) % attackHistory.Length;
        }
    }

    private IEnumerator RadialEmit()
    {
        enemy.canMove = false;
        enemy.anim.animator.SetTrigger("IsCasting");
        yield return new WaitForSeconds(0.3f);
        SpawnAttack(
            radialEmitter,
            enemy.target,
            Vector3.up,
            Vector3.up
        );
        yield return new WaitForSeconds(0.5f);

        AttackWarning();
        enemy.anim.animator.SetBool("IsReadyingAttack", true);
        yield return new WaitForSeconds(0.4f);
        enemy.anim.animator.SetBool("IsReadyingAttack", false);
        enemy.anim.animator.SetBool("IsAttacking", true);
        SpawnAttack(localSlam, enemy.target, enemy.toTargetDirection, enemy.toTargetDirection);
        yield return new WaitForSeconds(0.8f);

        enemy.canMove = true;
        enemy.isAttacking = false;
        enemy.anim.animator.SetBool("IsAttacking", false);
        AttackCooldown();
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
        yield return new WaitForSeconds(0.4f);
        enemy.anim.animator.SetBool("IsReadyingAttack", false);
        enemy.anim.animator.SetBool("IsAttacking", true);
        SpawnAttack(meleeAttack, enemy.target, direction, direction);
        yield return new WaitForSeconds(0.8f);

        AttackWarning();
        direction = enemy.toTargetDirection;
        enemy.move.Dash(direction, 3.0f);
        AttackZoneManager.Instance.SetSquareAttackZone(
            transform.position + direction * dist,
            direction,
            3.0f,
            2.0f * dist,
            1.0f
        );
        enemy.anim.animator.SetBool("IsReadyingAttack", true);
        yield return new WaitForSeconds(0.2f);
        enemy.anim.animator.SetBool("IsReadyingAttack", false);
        enemy.anim.animator.SetBool("IsAttacking", true);
        SpawnAttack(meleeAttack, enemy.target, direction, direction);
        yield return new WaitForSeconds(0.2f);

        AttackWarning();
        direction = enemy.toTargetDirection;
        enemy.move.Dash(direction, 2.0f);
        AttackZoneManager.Instance.SetSquareAttackZone(
            transform.position + direction * dist,
            direction,
            3.0f,
            2.0f * dist,
            1.0f
        );
        enemy.anim.animator.SetBool("IsReadyingAttack", true);
        yield return new WaitForSeconds(0.2f);
        
        enemy.anim.animator.SetBool("IsReadyingAttack", false);
        enemy.anim.animator.SetBool("IsAttacking", true);
        SpawnAttack(meleeAttack, enemy.target, direction, direction);
        yield return new WaitForSeconds(0.8f);

        enemy.canMove = true;
        enemy.isAttacking = false;
        enemy.anim.animator.SetBool("IsAttacking", false);
        AttackCooldown();
    }
    private IEnumerator Shoot()
    {
        GameObject shootTarget = enemy.target;
        if (shootTarget != null)
        {
            enemy.anim.animator.SetTrigger("IsCasting");
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
            enemy.anim.animator.ResetTrigger("IsCasting");
            enemy.anim.animator.SetTrigger("IsCasting");
        }

        enemy.isAttacking = false;
        AttackCooldown();
    }
}
