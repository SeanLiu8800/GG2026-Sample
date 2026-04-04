using UnityEngine;

public class BulletLockToOwner : BulletComponent
{
    private void Start()
    {
        if (bullet.owner == null) Debug.LogWarning($"{this.name} DOES NOT have an Owner!");
    }
    void Update()
    {
        if (bullet.owner != null) this.transform.position = bullet.owner.transform.position;
    }
}
