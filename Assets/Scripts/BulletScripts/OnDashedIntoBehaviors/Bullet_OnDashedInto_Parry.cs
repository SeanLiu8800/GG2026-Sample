using UnityEngine;

public class Bullet_OnDashedInto_Parry : Bullet_OnDashedInto_BehaviorBase
{
    protected override void OnDashedInto(Player player)
    {
        player.playerEvents.onParry?.Invoke();

        if (bullet.owner == null || !bullet.owner.TryGetComponent<Enemy>(out Enemy enemy)) return;
        enemy.enemyEvents.onParried?.Invoke(player.gameObject, bullet.GetParry());
    }
}
