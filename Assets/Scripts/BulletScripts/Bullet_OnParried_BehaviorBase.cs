public abstract class Bullet_OnParried_BehaviorBase : BulletComponent
{
    protected void OnEnable()
    {
        bullet.bulletEvents.onParried += onParried;
    }
    protected void OnDisable()
    {
        bullet.bulletEvents.onParried -= onParried;
    }
    protected void onParried()
    {
        bullet.bulletEvents.onParried -= onParried;
        onParriedBehavior();
    }
    protected abstract void onParriedBehavior();
}
