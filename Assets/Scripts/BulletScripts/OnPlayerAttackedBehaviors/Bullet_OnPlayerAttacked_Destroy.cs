using UnityEngine;

public class Bullet_OnPlayerAttacked_Destroy : Bullet_OnPlayerAttacked_BehaviorBase
{
    protected override void OnPlayerAttackedBehavior(Player player)
    {
        Destroy(this.gameObject);
    }
}
