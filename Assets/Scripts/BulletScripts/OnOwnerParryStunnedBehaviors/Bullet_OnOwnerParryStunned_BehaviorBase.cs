using UnityEngine;

public abstract class Bullet_OnOwnerParryStunned_BehaviorBase : BulletComponent
{
    [SerializeField, ReadOnly] protected Enemy ownerEnemy;
    protected void OnEnable()
    {
        SubscribeToEnemy();
    }
    protected void OnDisable()
    {
        UnsubscribeToEnemy();
    }
    protected void Start()
    {
        if (!bullet.owner.TryGetComponent<Enemy>(out Enemy _ownerEnemy))
        {
            Debug.LogWarning("This OnOwnerParryStunned Behavior doesn't have an Enemy as it's Owner! Disabling!");
            this.enabled = false;
            return;
        }

        ownerEnemy = _ownerEnemy;
        SubscribeToEnemy();
    }
    protected void SubscribeToEnemy()
    {
        if (ownerEnemy == null) return;

        ownerEnemy.enemyEvents.parryStunStarts += OnOwnerParryStunned;
    }
    protected void UnsubscribeToEnemy()
    {
        if (ownerEnemy == null) return;

        ownerEnemy.enemyEvents.parryStunStarts -= OnOwnerParryStunned;
    }
    protected abstract void OnOwnerParryStunned();
}