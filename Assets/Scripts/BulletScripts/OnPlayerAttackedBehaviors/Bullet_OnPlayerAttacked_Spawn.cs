using UnityEngine;

public class Bullet_OnPlayerAttacked_Spawn : Bullet_OnPlayerAttacked_BehaviorBase
{
    [SerializeField] private GameObject spawnObject;

    private void Start()
    {
        if (spawnObject == null)
        {
            Debug.LogError("No spawnObject is set! Removing Component!");
            Destroy(this);
        }
    }
    protected override void OnPlayerAttackedBehavior(Player player)
    {
        GameObject spawnedObject = Instantiate(spawnObject, this.transform.position, this.transform.rotation);
        if (spawnedObject.TryGetComponent<BulletScript>(out BulletScript bullet))
        {
            bullet.Initialize(bullet.target, bullet.owner, bullet.moveDirection, bullet.moveDirection);
        }
    }
}
