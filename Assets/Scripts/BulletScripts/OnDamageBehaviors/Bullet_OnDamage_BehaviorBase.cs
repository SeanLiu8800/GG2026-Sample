using UnityEngine;
public abstract class Bullet_OnDamage_BehaviorBase : BulletComponent
{
    protected virtual void OnEnable()
    {
        bullet.bulletEvents.onDamage += OnDamage;
    }
    protected virtual void OnDisable()
    {
        bullet.bulletEvents.onDamage -= OnDamage;
    }
    protected abstract void OnDamage(GameObject hitObject);
}
