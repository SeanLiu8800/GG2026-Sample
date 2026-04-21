using UnityEngine;

public class BulletLockToTarget : BulletComponent
{
    [SerializeField] private Vector3 offset = Vector3.zero;
    private void Start()
    {
        if (bullet.owner == null) Debug.LogWarning($"{this.name} DOES NOT have a Target!");
        else transform.position = bullet.target.transform.position + offset;
    }
    void Update()
    {
        if (bullet.target != null) transform.position = bullet.target.transform.position + offset;
    }
}
