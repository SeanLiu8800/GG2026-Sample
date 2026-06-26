using UnityEngine;

public class Bullet_OnLandedEnhancedAttack_DrainEngine : Bullet_OnLandedEnhancedAttack_BehaviorBase
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
    protected override void OnLandedEnhancedAttack(BulletScript bullet)
    {
        player.attack.DrainEngine(bullet.engineDrainAmount);
        if (onlyApplyOnce) bullet.bulletEvents.onLandedEnhancedAttack -= OnLandedEnhancedAttack;
    }
}
