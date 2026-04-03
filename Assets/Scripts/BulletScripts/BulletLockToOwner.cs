using UnityEngine;

public class BulletLockToOwner : BulletComponent
{
    void Update()
    {
        this.transform.position = bullet.owner.transform.position;
    }
}
