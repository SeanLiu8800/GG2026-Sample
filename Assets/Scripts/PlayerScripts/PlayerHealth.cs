using UnityEngine;

public class PlayerHealth : PlayerComponent, IDamageable
{
    [field : SerializeField] public int maxHealth { get; private set; } = 5;
    [field : SerializeField] public int currHealth { get; private set; } = 5;

    public void Damage(int damage = 1)
    {
        if (damage < 0)
        {
            Heal(-damage);
            return;
        }

        Debug.Log("Player Takes Damage");
        currHealth = Mathf.Clamp(currHealth - damage, 0, maxHealth);
        if (currHealth <= 0) Die();

        return;
    }
    public void Heal(int heal = 1)
    {
        if (heal < 0)
        {
            Damage(-heal);
            return;
        }

        Debug.Log("Player Heals");
        currHealth = Mathf.Clamp(currHealth + heal, 0, maxHealth);

        return;
    }
    public void Die()
    {
        Debug.Log($"{this.name} has run out of health!");
    }
}
