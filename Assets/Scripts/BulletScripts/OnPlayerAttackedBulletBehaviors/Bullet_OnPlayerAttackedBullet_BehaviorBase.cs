using UnityEngine;

public abstract class Bullet_OnPlayerAttackedBullet_BehaviorBase : BulletComponent
{
    [Tooltip("Determines whether this behavior only applies once, or multiple times when multiple bullets are Player Attacked by this bullet")]
    [SerializeField] protected bool onlyApplyOnce = true;
    protected virtual void OnEnable()
    {
        bullet.bulletEvents.onPlayerAttackedBullet += OnPlayerAttackedBullet;
    }
    protected virtual void OnDisable()
    {
        bullet.bulletEvents.onPlayerAttackedBullet -= OnPlayerAttackedBullet;
    }
    protected abstract void OnPlayerAttackedBullet(BulletScript bullet);
}
