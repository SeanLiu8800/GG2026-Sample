using UnityEngine;

public class EnemyHealth : EnemyComponent, IDamageable
{
    [SerializeField] private LayerMask layerMask;
    [field: SerializeField] public int maxHealth { get; private set; } = 5;
    [field: SerializeField] public int currHealth { get; private set; } = 5;

    public void Damage(int damage = 1)
    {
        if (damage < 1)
        {
            Heal(-damage);
            return;
        }

        Debug.Log("Enemy Takes Damage");
        currHealth = Mathf.Clamp(currHealth - damage, 0, maxHealth);
        if (currHealth <= 0) Die();

        return;
    }
    public void Heal(int heal = 1)
    {
        if (heal < 1)
        {
            Damage(-heal);
            return;
        }

        Debug.Log("Enemy Heals");
        currHealth = Mathf.Clamp(currHealth + heal, 0, maxHealth);

        return;
    }
    public void Die()
    {
        Debug.Log("Enemy runs out of health!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & layerMask) == 0) return;

        Player player = collision.gameObject.GetComponentInParent<Player>();
        if (player == null) return;

        Damage(player.attack.currDamage);
    }
}
