using UnityEngine;

public class BulletCollision : BulletComponent
{
    private void Start()
    {
        if (bullet.interactLayer == 0) Debug.LogWarning($"{this.name}'s layerMask is set to Nothing! Should you set this to something?");
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & bullet.interactLayer) == 0) return;

        // If Bullet hits Player or Player's Attack area
        if (TryCheckIfPlayerCollider(collision, out Player player))
        {
            PlayerCollision(player);
        }
        else if (collision.TryGetComponent(out IDamageable iDamageable))
        {
            iDamageable.Damage(bullet.damage);
            Destroy(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    /// <summary>
    /// Checks whether input Collider2D is from the Player
    /// </summary>
    /// <param name="collision">The Collider2D to check</param>
    /// <param name="player">The corresponding Player script from the Collider</param>
    /// <returns>
    /// Returns True and the corresponding Player script if the Collider2D is from the Player<br/>
    /// Returns False otherwise
    /// </returns>
    private bool TryCheckIfPlayerCollider(Collider2D collision, out Player player)
    {
        // If collider is directly from Player GameObject
        if (collision.TryGetComponent<Player>(out player)) return true;
        // If collider is from Player's Attack Area
        else if (collision.transform.parent != null && collision.transform.parent.TryGetComponent<Player>(out player)) return true;
        else return false;
    }
    /// <summary>
    /// Performs logic when Player collides with this bullet
    /// </summary>
    /// <param name="player">Player to attack</param>
    /// <returns>Returns True if the bullet interacts with Player, False otherwise (like if Player is Invincible)</returns>
    private void PlayerCollision(Player player)
    {
        if (player.move.isDashing) bullet.bulletEvents.onDashedInto?.Invoke(player);
        else if (player.attack.isAttacking)
        {
            if (player.attack.attackIsEnhanced) bullet.bulletEvents.onEnhancedAttacked?.Invoke(player);
        }
        else
        {
            if (!player.health.isInvincible)
            {
                player.health.Damage(bullet.damage);
                bullet.bulletEvents.onDamagePlayer?.Invoke(player);
            }
        }
    }
}
