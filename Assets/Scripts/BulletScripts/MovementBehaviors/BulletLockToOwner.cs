using UnityEngine;

public class BulletLockToOwner : BulletComponent
{
    [SerializeField] private float distanceOffset = 0.0f;
    [SerializeField] private Vector3 relativeOffset = Vector3.zero;
    private void Start()
    {
        if (bullet.owner == null) Debug.LogWarning($"{this.name} DOES NOT have an Owner!");
        else ApplyOffset();
    }
    void Update()
    {
        if (bullet.owner != null) ApplyOffset();
    }
    private void ApplyOffset()
    {
        transform.position = bullet.owner.transform.position + 
            (bullet.moveDirection * distanceOffset) + relativeOffset;
    }
}
