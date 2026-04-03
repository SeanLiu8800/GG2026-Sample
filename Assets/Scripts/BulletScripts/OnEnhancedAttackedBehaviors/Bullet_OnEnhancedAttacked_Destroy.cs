using UnityEngine;

public class Bullet_OnEnhancedAttacked_Destroy : Bullet_OnEnhancedAttacked_BehaviorBase
{
    protected override void OnEnhancedAttackedBehavior(Player player)
    {
        Destroy(this.gameObject);
    }
}
