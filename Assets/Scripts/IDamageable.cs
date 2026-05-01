using UnityEngine;
using System.Collections;
public interface IDamageable
{
    public float currHealth { get; }
    public float maxHealth { get; }
    public float fireBuildup { get; }
    public float fireLimit { get; }
    public float iceBuildup { get; }
    public float iceLimit { get; }
    public float shockBuildup { get; }
    public float shockLimit { get; }

    /// <summary>Process the bullet that hits this GameObject</summary>
    /// <param name="bullet">The Bullet that does the damage</param>
    public void BulletHits(BulletScript bullet);
    /// <summary>Damages this GameObject by a specific amount</summary>
    /// <param name="damage">The amount of Damage received</param>
    /// <param name="damager">The GameObject that does the damage</param>
    public void Damage(
        float damage, 
        DamageElement element = DamageElement.None, 
        float elementBuildup = 0.0f, 
        GameObject damager = null
    );
    public void ElementDamage(DamageElement element, float buildupRate);

    /// <summary>Heals this GameObject by a specific amount</summary>
    /// <param name="heal">The amount of Healing received</param>
    /// <param name="healer">The GameObject that does the healing</param>
    public void Heal(float heal, GameObject healer = null);

    public void Die();

    public IEnumerator Afterburn();
    public IEnumerator Corrosion();
}

public enum DamageElement { None, Fire, Ice, Shock }