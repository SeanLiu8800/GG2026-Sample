public interface IDamageable
{
    public float currHealth { get; }
    public float maxHealth { get; }
    public void Damage(float damage = 1.0f);
    public void Heal(float heal = 1.0f);
    public void Die();
}
