using UnityEngine;

public class BulletGrab : Bullet_OnDamage_BehaviorBase
{
    protected override void OnDamage(GameObject hitObject)
    {
        if (!hitObject.TryGetComponent<Player>(out Player player))
        {
            Debug.Log("Object is NOT a Player!");
        }
        else
        {
            Debug.Log("Object IS a Player!");
        }
    }
}
