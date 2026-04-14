using UnityEngine;

public class RoomBullets : RoomComponent
{
    public GameObject bulletContainer { get; private set; }
    protected override void Awake()
    {
        base.Awake();

        bulletContainer = new GameObject("BulletContainer");
        bulletContainer.transform.parent = this.transform;
        bulletContainer.isStatic = true;
    }
    public void DeleteAllBullets()
    {
        foreach (Transform childTransform in bulletContainer.transform) Destroy(childTransform.gameObject);
    }
}
