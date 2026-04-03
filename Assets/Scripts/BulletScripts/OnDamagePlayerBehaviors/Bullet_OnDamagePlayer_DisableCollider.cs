using UnityEngine;

public class Bullet_OnDamagePlayer_DisableCollider : Bullet_OnDamagePlayer_BehaviorBase
{
    protected override void OnDamagePlayer(Player player)
    {
        bullet.bulletCollider.enabled = false;
    }
}
