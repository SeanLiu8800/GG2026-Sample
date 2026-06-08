using UnityEngine;

public class Bullet_OnEnhancedAttacked_BeParried : Bullet_OnEnhancedAttacked_BehaviorBase
{
    protected override void OnEnhancedAttackedBehavior(Player player)
    {
        player.playerEvents.onParry?.Invoke();

        if (bullet.owner == null || !bullet.owner.TryGetComponent<Enemy>(out Enemy enemy)) return;
        enemy.enemyEvents.onParried?.Invoke(player.gameObject);
    }
}
