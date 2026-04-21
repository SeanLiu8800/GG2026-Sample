using UnityEngine;

public class EnemyHealth : EnemyComponent, IDamageable
{
    [field: SerializeField] public float currHealth { get; private set; } = 5;
    [field: SerializeField] public float maxHealth { get; private set; } = 5;
    protected virtual void OnEnable()
    {
        enemy.enemyEvents.enemyDies += EnemyDies;
    }
    protected virtual void OnDisable()
    {
        enemy.enemyEvents.enemyDies -= EnemyDies;
    }

    #region ----- Event Functions -----
    protected virtual void EnemyDies()
    {
        enemy.spriteRenderer.color = Color.white;
        enemy.spriteRenderer.SetAlpha(0.2f);
        enemy.enemyCollider.excludeLayers = LayerMask.GetMask("Player", "Enemy");
    }
    #endregion
    public void Damage(float damage, BulletScript bullet)
    {
        if (damage < 0.0f)
        {
            Heal(-damage, bullet.gameObject);
            return;
        }
        if (!enemy.allowDamage) return;

        bullet.bulletEvents.onDamage?.Invoke(this.gameObject);
        Damage(damage, bullet.gameObject);
    }
    public void Damage(float damage, GameObject damager = null)
    {
        if (damage < 0.0f)
        {
            Heal(-damage, damager);
            return;
        }
        if (!enemy.allowDamage) return;
        
        float originalHealth = currHealth;
        currHealth = Mathf.Clamp(currHealth - damage, 0.0f, maxHealth);

        if (originalHealth != currHealth) enemy.enemyEvents.onHealthChange?.Invoke();
        enemy.enemyEvents.onDamage?.Invoke();

        AudioManager.Instance.PlaySoundOneShot(AudioManager.Instance.soundEffects.enemyHurts);
        if (currHealth <= 0.0f) Die();

        return;
    }
    public void Heal(float heal, GameObject healer = null)
    {
        if (heal < 0.0f)
        {
            Damage(-heal, healer);
            return;
        }

        float originalHealth = currHealth;
        currHealth = Mathf.Clamp(currHealth + heal, 0.0f, maxHealth);
        if (originalHealth != currHealth) enemy.enemyEvents.onHealthChange?.Invoke();
        enemy.enemyEvents.onHeal?.Invoke();

        return;
    }
    public void Die()
    {
        enemy.enemyEvents.onEnemyDies?.Invoke();
        enemy.enemyEvents.enemyDies?.Invoke();
    }

    private int lastAttackID = 0;
    private void OnTriggerStay2D(Collider2D collision)
    {
        // Only tracks player attack colliders!
        if (collision.gameObject.layer == enemy.playerLayer) return;

        Player player = collision.gameObject.GetComponentInParent<Player>();
        if (player == null || !player.attack.isAttacking) return;
        if (lastAttackID == player.attack.currAttackID) return;
        
        lastAttackID = player.attack.currAttackID;
        Damage(player.attack.currDamage);
        enemy.move.Dash((transform.position - collision.transform.position).normalized, 1.0f);
    }
}