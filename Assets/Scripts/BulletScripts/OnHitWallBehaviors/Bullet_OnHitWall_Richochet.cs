using UnityEngine;

public class Bullet_OnHitWall_Richochet : Bullet_OnHitWall_BehaviorBase
{
    protected override void OnHitWall(Vector3 normalVector)
    {
        bullet.moveDirection = Vector3.Reflect(bullet.moveDirection, normalVector);
    }
}
