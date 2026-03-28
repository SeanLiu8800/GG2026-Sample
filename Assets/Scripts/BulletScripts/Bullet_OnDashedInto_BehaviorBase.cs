using UnityEngine;

public abstract class Bullet_OnDashedInto_BehaviorBase : BulletComponent
{
    protected void OnEnable()
    {
        bullet.bulletEvents.onDashedInto += OnDashedInto;
    }
    protected void OnDisable()
    {
        bullet.bulletEvents.onDashedInto -= OnDashedInto;
    }
    protected abstract void OnDashedInto(Player player);
}
