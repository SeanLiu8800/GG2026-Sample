using UnityEngine;

public abstract class Enemy_OnDeath_BehaviorBase : EnemyComponent
{
    private void OnEnable()
    {
        enemy.enemyEvents.onEnemyDies += OnDeath;
    }
    private void OnDisable()
    {
        enemy.enemyEvents.onEnemyDies -= OnDeath;
    }

    protected abstract void OnDeath();
}
