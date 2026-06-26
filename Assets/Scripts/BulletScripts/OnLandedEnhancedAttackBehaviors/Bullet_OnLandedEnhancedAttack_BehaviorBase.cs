using UnityEngine;

public abstract class Bullet_OnLandedEnhancedAttack_BehaviorBase : BulletComponent
{
    [Tooltip("Determines whether this behavior only applies once, or multiple times when multiple bullets are Enhanced Attacked by this bullet")]
    [SerializeField] protected bool onlyApplyOnce = true;
    protected virtual void OnEnable()
    {
        bullet.bulletEvents.onLandedEnhancedAttack += OnLandedEnhancedAttack;
    }
    protected virtual void OnDisable()
    {
        bullet.bulletEvents.onLandedEnhancedAttack -= OnLandedEnhancedAttack;
    }
    protected abstract void OnLandedEnhancedAttack(BulletScript bullet);
}
