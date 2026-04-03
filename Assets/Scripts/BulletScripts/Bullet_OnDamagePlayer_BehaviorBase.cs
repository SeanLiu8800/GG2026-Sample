public abstract class Bullet_OnDamagePlayer_BehaviorBase : BulletComponent
{
    protected virtual void OnEnable()
    {
        bullet.bulletEvents.onDamagePlayer += OnDamagePlayer;
    }
    protected virtual void OnDisable()
    {
        bullet.bulletEvents.onDamagePlayer -= OnDamagePlayer;
    }
    protected abstract void OnDamagePlayer(Player player);
}
