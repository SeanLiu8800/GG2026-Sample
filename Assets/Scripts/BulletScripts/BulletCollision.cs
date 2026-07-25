using UnityEngine;
using System.Collections.Generic;

public class BulletCollision : BulletComponent
{
    private int wallLayer;
    [SerializeField] private Dictionary<GameObject, int> damagedGameObjects;
    [Tooltip("Controls how many times this same exact bullet can hit the same GameObject")]
    [SerializeField, Range(0, 5)] private int perTargetHitCount = 1;
    private void Start()
    {
        if (bullet.damageLayer == 0) Debug.LogWarning($"{this.name}'s layerMask is set to Nothing! Should you set this to something?");
        if ((wallLayer = LayerMask.NameToLayer("Wall")) == 0) Debug.LogError("COULD NOT find Wall Layer!");

        damagedGameObjects = new Dictionary<GameObject, int>(10);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == wallLayer)
        {
            Vector3 normalVector = Physics2D.Raycast(transform.position, bullet.moveDirection).normal;
            bullet.bulletEvents.onHitWall?.Invoke(normalVector);
        }
        if (((1 << collision.gameObject.layer) & bullet.damageLayer) == 0) return;
        // If Bullet hits a Damageable GameObject
        IDamageable damageable = collision.GetComponentInParent<IDamageable>();
        bool hasHitBefore = damagedGameObjects.TryGetValue(collision.gameObject, out int hitCount);
        if (damageable != null && (!hasHitBefore || hitCount < perTargetHitCount))
        {
            if (!hasHitBefore) damagedGameObjects.Add(collision.gameObject, 1);
            else damagedGameObjects[collision.gameObject]++;

            damageable.BulletHits(bullet);
        }
        //else Debug.LogWarning($"Bullet hits {collision.name}, which isn't damageable!");
    }
}
