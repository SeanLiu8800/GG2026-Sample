public abstract class Bullet_OnPlayerAttacked_BehaviorBase : BulletComponent
{
    protected void OnEnable()
    {
        bullet.bulletEvents.onPlayerAttacked += OnPlayerAttacked;
    }
    protected void OnDisable()
    {
        bullet.bulletEvents.onPlayerAttacked -= OnPlayerAttacked;
    }
    protected void OnPlayerAttacked(Player player)
    {
        bullet.bulletEvents.onPlayerAttacked -= OnPlayerAttacked;
        OnPlayerAttackedBehavior(player);
    }
    protected abstract void OnPlayerAttackedBehavior(Player player);
}
