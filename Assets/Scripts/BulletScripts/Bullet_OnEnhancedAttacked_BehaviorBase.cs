public abstract class Bullet_OnEnhancedAttacked_BehaviorBase : BulletComponent
{
    protected void OnEnable()
    {
        bullet.bulletEvents.onEnhancedAttacked += OnEnhancedAttacked;
    }
    protected void OnDisable()
    {
        bullet.bulletEvents.onEnhancedAttacked -= OnEnhancedAttacked;
    }
    protected void OnEnhancedAttacked()
    {
        bullet.bulletEvents.onEnhancedAttacked -= OnEnhancedAttacked;
        OnEnhancedAttackedBehavior();
    }
    protected abstract void OnEnhancedAttackedBehavior();
}
