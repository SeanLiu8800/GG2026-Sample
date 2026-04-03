using UnityEngine;

public class BulletMovement : BulletComponent
{
    private void Start()
    {
        bullet.bulletRigidbody.linearVelocity = bullet.initialLinearVelocity;
    }
}
