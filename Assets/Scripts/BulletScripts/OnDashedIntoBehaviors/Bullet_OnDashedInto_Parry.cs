using UnityEngine;

public class Bullet_OnDashedInto_Parry : Bullet_OnDashedInto_BehaviorBase
{
    [Header("Dash Parry Variable")]
    [SerializeField, Range(0.0f, 2.0f)] private float parryMultiplier = 0.75f;
    protected override void OnDashedInto(Player player)
    {
        player.playerEvents.onParry?.Invoke();

        if (bullet.owner == null || !bullet.owner.TryGetComponent<Enemy>(out Enemy enemy)) return;
        enemy.enemyEvents.onParried?.Invoke(player.gameObject, bullet.GetParry() * parryMultiplier);
    }
}
