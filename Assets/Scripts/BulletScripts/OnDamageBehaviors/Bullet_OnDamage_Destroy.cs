using UnityEngine;

public class Bullet_OnDamage_Destroy : Bullet_OnDamage_BehaviorBase
{
    protected override void OnDamage(GameObject hitTarget)
    {
        Destroy(this.gameObject);
    }
}
