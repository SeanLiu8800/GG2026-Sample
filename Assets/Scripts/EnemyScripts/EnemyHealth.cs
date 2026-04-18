using UnityEngine;

public class EnemyHealth : EnemyComponent, IDamageable
{
    [field: SerializeField] public float currHealth { get; private set; } = 5;
    [field: SerializeField] public float maxHealth { get; private set; } = 5;
    protected virtual void OnEnable()
    {
        enemy.enemyEvents.onEnemyDies += OnEnemyDies;
    }
    protected virtual void OnDisable()
    {
        enemy.enemyEvents.onEnemyDies -= OnEnemyDies;
    }

    #region ----- Event Functions -----
    protected virtual void OnEnemyDies()
    {
        enemy.spriteRenderer.color = Color.white;
        enemy.spriteRenderer.SetAlpha(0.2f);
        enemy.enemyCollider.excludeLayers = enemy.playerLayer;
        //enemy.enemyCollider.enabled = false;
        //Destroy(this.gameObject);
        //Invoke(nameof(Respawn), 1.0f);
    }
    #endregion
    
    public void Damage(float damage = 1.0f)
    {
        if (!enemy.allowDamage) return;
        if (damage < 0.0f)
        {
            Heal(-damage);
            return;
        }
        float originalHealth = currHealth;
        currHealth = Mathf.Clamp(currHealth - damage, 0.0f, maxHealth);

        if (originalHealth != currHealth) enemy.enemyEvents.onHealthChange?.Invoke();
        enemy.enemyEvents.onDamage?.Invoke();

        AudioManager.Instance.PlaySoundOneShot(AudioManager.Instance.soundEffects.enemyHurts);
        if (currHealth <= 0.0f) Die();

        return;
    }
    public void Heal(float heal = 1.0f)
    {
        if (heal < 0.0f)
        {
            Damage(-heal);
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
    private void Respawn()
    {
        enemy.spriteRenderer.enabled = true;
        enemy.enemyCollider.enabled = true;
        Heal(99);
        this.transform.position = new Vector3(3.5f, 0.0f, 0.0f);
    }

    private int lastAttackID = 0;
    private void OnTriggerStay2D(Collider2D collision)
    {
        // Only tracks player attack colliders!
        if (((1 << collision.gameObject.layer) & enemy.playerLayer) == 0) return;

        Player player = collision.gameObject.GetComponentInParent<Player>();
        if (player == null || !player.attack.isAttacking) return;
        if (lastAttackID == player.attack.currAttackID) return;
        
        lastAttackID = player.attack.currAttackID;
        Damage(player.attack.currDamage);
        enemy.move.Dash((transform.position - collision.transform.position).normalized, 1.0f);
    }
}