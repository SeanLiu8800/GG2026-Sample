using UnityEngine;

public class BulletLockToOwner : BulletComponent
{
    [SerializeField] private Vector3 offset = Vector3.zero;
    private void Start()
    {
        if (bullet.owner == null) Debug.LogWarning($"{this.name} DOES NOT have an Owner!");
        else transform.position = bullet.owner.transform.position + offset;
    }
    void Update()
    {
        if (bullet.owner != null) transform.position = bullet.owner.transform.position + offset;
    }
}
