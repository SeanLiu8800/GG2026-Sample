using UnityEngine;

public class Bullet_OnDamage_DisableCollider : Bullet_OnDamage_BehaviorBase
{
    protected override void OnDamage(GameObject hitTarget)
    {
        bullet.bulletCollider.enabled = false;
    }
}
