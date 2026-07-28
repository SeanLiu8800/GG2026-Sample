using UnityEngine;
using System.Collections.Generic;
public class Bullet_IsPlayerAttack : BulletComponent
{
    private Player connectedPlayer;
    private int attackLayer;

    private Dictionary<BulletScript, int> attackedBullets;
    private void Start()
    {
        if (bullet.owner == null) Debug.LogWarning("This bullet DOES NOT have an Owner!");
        if (!bullet.owner.TryGetComponent<Player>(out connectedPlayer))
        {
            Debug.LogWarning("Bullet Owner IS NOT a Player! Disabling this component!");
            this.enabled = false;
        }

        if ((attackLayer = LayerMask.NameToLayer("Attack")) == 0) Debug.LogError("Attack Layer NOT FOUND!");

        attackedBullets = new Dictionary<BulletScript, int>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != attackLayer) return;
        if (!collision.TryGetComponent<BulletScript>(out BulletScript hitBullet))
        {
            Debug.Log($"{collision.name} DOES NOT have a BulletScript Component!");
            return;
        }

        // Check if struck bullet was player attacked previously
        bool hasHitBefore = attackedBullets.TryGetValue(hitBullet, out int hitCount);
        if (!hasHitBefore)
        {
            //Record the player attacked bullet so this bullet cannot be parry it again
            attackedBullets.Add(hitBullet, 1);
            bullet.bulletEvents.onPlayerAttacksBullet?.Invoke(hitBullet);
            hitBullet.bulletEvents.onPlayerAttacked?.Invoke(connectedPlayer);
        }
    }
}
