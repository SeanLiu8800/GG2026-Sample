using UnityEngine;

public class EnemyAnimation : EnemyComponent
{
    private Animator animator;
    protected override void Awake()
    {
        base.Awake();
        if (!TryGetComponent<Animator>(out Animator _animator)) Debug.LogError($"{this.name} DOES NOT have an Animator Component!");
        animator = _animator;
    }

    protected void OnEnable()
    {
        enemy.enemyEvents.parryStunStarts += ParryStunStarts;
        enemy.enemyEvents.parryStunEnds += ParryStunEnds;
    }
    protected void OnDisable()
    {
        enemy.enemyEvents.parryStunStarts -= ParryStunStarts;
        enemy.enemyEvents.parryStunEnds -= ParryStunEnds;
    }
    #region ----- Event Functions -----
    void ParryStunStarts()
    {
        animator.SetBool("parryStunned", true);
    }
    void ParryStunEnds()
    {
        animator.SetBool("parryStunned", false);
    }
    #endregion
}
