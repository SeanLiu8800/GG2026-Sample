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
    protected void OnEnhancedAttacked(Player player)
    {
        bullet.bulletEvents.onEnhancedAttacked -= OnEnhancedAttacked;
        OnEnhancedAttackedBehavior(player);
    }
    protected abstract void OnEnhancedAttackedBehavior(Player player);
}
