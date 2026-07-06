using UnityEngine;

public class Bullet_OnPlayerAttacked_BeParried : Bullet_OnPlayerAttacked_BehaviorBase
{
    protected override void OnPlayerAttackedBehavior(Player player)
    {
        player.playerEvents.onParry?.Invoke();

        if (bullet.owner == null || !bullet.owner.TryGetComponent<Enemy>(out Enemy enemy)) return;
        enemy.enemyEvents.onParried?.Invoke(player.gameObject);
    }
}
