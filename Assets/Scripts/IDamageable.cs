using UnityEngine;
public interface IDamageable
{
    public float currHealth { get; }
    public float maxHealth { get; }

    /// <summary>Damages this GameObject by a specific amount</summary>
    /// <param name="damage">The amount of Damage received</param>
    /// <param name="bullet">The Bullet that does the damage</param>
    public void Damage(float damage, BulletScript bullet);
    /// <summary>Damages this GameObject by a specific amount</summary>
    /// <param name="damage">The amount of Damage received</param>
    /// <param name="damager">The GameObject that does the damage</param>
    public void Damage(float damage, GameObject damager = null);

    /// <summary>Heals this GameObject by a specific amount</summary>
    /// <param name="heal">The amount of Healing received</param>
    /// <param name="healer">The GameObject that does the healing</param>
    public void Heal(float heal, GameObject healer = null);

    public void Die();
}
