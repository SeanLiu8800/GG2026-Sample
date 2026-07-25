using UnityEngine;

public class Bullet_OnPlayerAttacksBullet_DrainEngine : Bullet_OnPlayerAttacksBullet_BehaviorBase
{
    private Player player;

    private void Start()
    {
        if (!bullet.owner.TryGetComponent<Player>(out player))
        {
            Debug.LogError($"{this.name}'s owner DOES NOT have a Player Component! Disabling!");
            this.enabled = false;
        }
    }
    protected override void OnPlayerAttacksBullet(BulletScript bullet)
    {
        player.attack.DrainEngine(bullet.bulletStats.engineDrainAmount, bullet.owner);
        if (onlyApplyOnce) bullet.bulletEvents.onPlayerAttacksBullet -= OnPlayerAttacksBullet;
    }
}
