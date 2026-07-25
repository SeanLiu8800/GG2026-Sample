using UnityEngine;

public class Bullet_Multiplier_DashDestroy : BulletComponent
{
    [SerializeField, Range(0, 10)] private int dashDestroyCount = 0;
    private void OnEnable()
    {
        bullet.bulletEvents.onPlayerAttacksBullet += OnPlayerAttacksBullet;
    }
    private void OnDisable()
    {
        bullet.bulletEvents.onPlayerAttacksBullet -= OnPlayerAttacksBullet;
    }
    protected void OnPlayerAttacksBullet (BulletScript hitBullet)
    {
        hitBullet.parryMultipliers.Add(new Bullet_StatMultiplier_Parry(dashDestroyCount));
    }
    private void Start()
    {
        bullet.damageMultipliers.Add(new Bullet_StatMultiplier_Damage(dashDestroyCount));
    }
    public void Initialize(int dashDestroyCount = 0)
    {
        this.dashDestroyCount = dashDestroyCount;
    }
}
