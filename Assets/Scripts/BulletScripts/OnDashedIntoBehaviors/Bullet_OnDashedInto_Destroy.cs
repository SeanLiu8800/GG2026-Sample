using UnityEngine;

public class Bullet_OnDashedInto_Destroy : Bullet_OnDashedInto_BehaviorBase
{
    protected override void OnDashedInto(Player player)
    {
        Destroy(this.gameObject);
    }
}
