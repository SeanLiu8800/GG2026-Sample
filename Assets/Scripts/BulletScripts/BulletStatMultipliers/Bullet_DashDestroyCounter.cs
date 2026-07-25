using UnityEngine;

public class Bullet_DashDestroyCounter : BulletComponent
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
    /// <summary>
    /// Add a multiplier to the hit bullet's parry value just incase it is parried when struck by a player's attack
    /// </summary>
    /// <param name="hitBullet">The bullet struck by the player's attack</param>
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
