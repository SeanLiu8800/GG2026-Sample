using UnityEngine;

public class Bullet_OnHitWall_Destroy : Bullet_OnHitWall_BehaviorBase
{
    protected override void OnHitWall(Vector3 normalVector)
    {
        Destroy(this.gameObject);
    }
}
