using UnityEngine;

public class Bullet_OnPlayerAttackedBullet_DrainEngine : Bullet_OnPlayerAttackedBullet_BehaviorBase
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
    protected override void OnPlayerAttackedBullet(BulletScript bullet)
    {
        player.attack.DrainEngine(bullet.engineDrainAmount, bullet.owner);
        if (onlyApplyOnce) bullet.bulletEvents.onPlayerAttackedBullet -= OnPlayerAttackedBullet;
    }
}
