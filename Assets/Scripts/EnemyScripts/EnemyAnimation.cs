using UnityEngine;
using UnityEngine.InputSystem;
public class EnemyAnimation : EnemyComponent
{
    [HideInInspector] public Animator animator;
    protected override void Awake()
    {
        base.Awake();
        if (!TryGetComponent<Animator>(out Animator _animator)) Debug.LogError($"{this.name} DOES NOT have an Animator Component!");
        animator = _animator;

        if (animator.runtimeAnimatorController == null)
        {
            Debug.LogWarning($"{this.name} DOES NOT have an Animator Controller! Disabling this EnemyAnimation Component");
            this.enabled = false;
        }
    }

    protected void OnEnable()
    {
        //enemy.enemyEvents.meleeAttack += MeleeAttack;
        enemy.enemyEvents.attackEnds += AttackEnds;

        enemy.enemyEvents.parryStunStarts += ParryStunStarts;
        enemy.enemyEvents.parryStunEnds += ParryStunEnds;
    }
    protected void OnDisable()
    {
        //enemy.enemyEvents.meleeAttack -= MeleeAttack;
        enemy.enemyEvents.attackEnds -= AttackEnds;

        enemy.enemyEvents.parryStunStarts -= ParryStunStarts;
        enemy.enemyEvents.parryStunEnds -= ParryStunEnds;
    }
    #region ----- Event Functions -----
    void MeleeAttack()
    {
        animator.SetBool("IsReadyingAttack", true);
    }
    void AttackEnds()
    {
        animator.SetBool("IsReadyingAttack", false);
        enemy.anim.animator.SetBool("IsAttacking", false);
    }
    void ParryStunStarts()
    {
        animator.SetBool("ParryStunned", true);
    }
    void ParryStunEnds()
    {
        animator.SetBool("ParryStunned", false);
    }
    #endregion

    private void Update()
    {
        UpdateMoveDirection();
        UpdateTargetDirection();
    }
    private void UpdateMoveDirection()
    {
        if (enemy.enemyRigidbody.linearVelocity == Vector2.zero) return;

        animator.SetFloat("LastMoveX", enemy.enemyRigidbody.linearVelocityX);
        animator.SetFloat("LastMoveY", enemy.enemyRigidbody.linearVelocityY);
    }
    private void UpdateTargetDirection()
    {
        Vector2 toTargetVector = enemy.toTargetVector;
        animator.SetFloat("AttackDirectionX", toTargetVector.x);
        animator.SetFloat("AttackDirectionY", toTargetVector.y);
    }
}
