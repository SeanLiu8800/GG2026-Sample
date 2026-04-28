using UnityEngine;

public class Bullet_IsDashAttack : BulletComponent
{
    private Player connectedPlayer;
    private int attackLayer;
    private void Start()
    {
        if (bullet.owner == null) Debug.LogWarning("This bullet DOES NOT have an Owner!");
        if (!bullet.owner.TryGetComponent<Player>(out connectedPlayer))
        {
            Debug.LogWarning("Bullet Owner IS NOT a Player! Disabling this component!");
            this.enabled = false;
        }

        if ((attackLayer = LayerMask.NameToLayer("Attack")) == 0) Debug.LogError("Attack Layer NOT FOUND!");
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != attackLayer) return;
        if (!collision.TryGetComponent<BulletScript>(out BulletScript bullet))
        {
            Debug.Log($"{collision.name} DOES NOT have a BulletScript Component!");
            return;
        }

        bullet.bulletEvents.onDashedInto?.Invoke(connectedPlayer);
    }
}
