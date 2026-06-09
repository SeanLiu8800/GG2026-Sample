using UnityEngine;

public abstract class Enemy_OnParryStun_BehaviorBase : EnemyComponent
{
    private void OnEnable()
    {
        enemy.enemyEvents.parryStunStarts += OnParryStunBehavior;
    }
    private void OnDisable()
    {
        enemy.enemyEvents.parryStunStarts -= OnParryStunBehavior;
    }
    protected abstract void OnParryStunBehavior();
}
