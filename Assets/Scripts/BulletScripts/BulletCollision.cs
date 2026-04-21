using UnityEngine;

public class BulletCollision : BulletComponent
{
    private int wallLayer;
    private void Start()
    {
        if (bullet.interactLayer == 0) Debug.LogWarning($"{this.name}'s layerMask is set to Nothing! Should you set this to something?");
        if ((wallLayer = LayerMask.NameToLayer("Wall")) == 0) Debug.LogError("COULD NOT find Wall Layer!");
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == wallLayer)
        {
            Vector3 normalVector = Physics2D.Raycast(transform.position, bullet.moveDirection).normal;
            bullet.bulletEvents.onHitWall?.Invoke(normalVector);
        }
        if (((1 << collision.gameObject.layer) & bullet.interactLayer) == 0) return;
        // If Bullet hits Player or Player's Attack area
        IDamageable damageable = collision.GetComponentInParent<IDamageable>();
        if (damageable != null && damageable.currHealth > 0.0f) damageable.Damage(bullet.damage, bullet);
        //else Debug.LogWarning($"Bullet hits {collision.name}, which isn't damageable!");
    }
}
