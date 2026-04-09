using UnityEngine;

public class EnemyHealth : EnemyComponent, IDamageable
{
    [SerializeField] private LayerMask damageLayer;
    [field: SerializeField] public int maxHealth { get; private set; } = 5;
    [field: SerializeField] public int currHealth { get; private set; } = 5;

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
        enemy.enemyCollider.enabled = false;
        //Destroy(this.gameObject);
        //Invoke(nameof(Respawn), 1.0f);
    }
    #endregion

    public void Damage(int damage = 1)
    {
        if (damage < 1)
        {
            Heal(-damage);
            return;
        }

        currHealth = Mathf.Clamp(currHealth - damage, 0, maxHealth);
        enemy.enemyEvents.onHealthChange?.Invoke();
        AudioManager.Instance.PlaySoundOneShot(AudioManager.Instance.soundEffects.enemyHurts);
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

        currHealth = Mathf.Clamp(currHealth + heal, 0, maxHealth);
        enemy.enemyEvents.onHealthChange?.Invoke();

        return;
    }
    public void Die()
    {
        enemy.enemyEvents.onEnemyDies?.Invoke();
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
        if (!enemy.allowDamage || ((1 << collision.gameObject.layer) & damageLayer) == 0) return;

        Player player = collision.gameObject.GetComponentInParent<Player>();
        if (player == null || !player.attack.isAttacking) return;
        if (lastAttackID == player.attack.currAttackID) return;
        
        lastAttackID = player.attack.currAttackID;
        Damage(player.attack.currDamage);
        enemy.enemyRigidbody.AddForce((transform.position - collision.transform.position).normalized * 10.0f, ForceMode2D.Impulse);
    }
}
