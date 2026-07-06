using UnityEngine;

public abstract class Bullet_OnPlayerAttacksBullet_BehaviorBase : BulletComponent
{
    [Tooltip("Determines whether this behavior only applies once, or multiple times when multiple bullets are Player Attacked by this bullet")]
    [SerializeField] protected bool onlyApplyOnce = true;
    protected virtual void OnEnable()
    {
        bullet.bulletEvents.onPlayerAttacksBullet += OnPlayerAttacksBullet;
    }
    protected virtual void OnDisable()
    {
        bullet.bulletEvents.onPlayerAttacksBullet -= OnPlayerAttacksBullet;
    }
    protected abstract void OnPlayerAttacksBullet(BulletScript bullet);
}
