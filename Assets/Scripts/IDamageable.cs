using UnityEngine;
public interface IDamageable
{
    public float currHealth { get; }
    public float maxHealth { get; }
    public void Damage(float damage, BulletScript bullet);
    public void Damage(float damage, GameObject damager = null);
    public void Heal(float heal, GameObject healer = null);
    public void Die();
}
