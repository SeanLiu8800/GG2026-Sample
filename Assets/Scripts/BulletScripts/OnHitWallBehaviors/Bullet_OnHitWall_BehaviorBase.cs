using UnityEngine;

public abstract class Bullet_OnHitWall_BehaviorBase : BulletComponent
{
    protected void OnEnable()
    {
        bullet.bulletEvents.onHitWall += OnHitWall;
    }
    protected void OnDisable()
    {
        bullet.bulletEvents.onHitWall -= OnHitWall;
    }
    protected abstract void OnHitWall(Vector3 normalVector);
}
