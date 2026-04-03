using UnityEngine;

public class Bullet_OnDamagePlayer_Destroy : Bullet_OnDamagePlayer_BehaviorBase
{
    protected override void OnDamagePlayer(Player player)
    {
        Destroy(this.gameObject);
    }
}
