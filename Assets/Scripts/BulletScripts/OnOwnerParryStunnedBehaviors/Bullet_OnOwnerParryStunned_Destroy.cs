using UnityEngine;

public class Bullet_OnOwnerParryStunned_Destroy : Bullet_OnOwnerParryStunned_BehaviorBase
{
    protected override void OnOwnerParryStunned()
    {
        Destroy(this.gameObject);
    }
}
