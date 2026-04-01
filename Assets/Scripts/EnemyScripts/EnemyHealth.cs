using UnityEngine;

public class EnemyHealth : EnemyComponent, IDamageable
{
    [SerializeField] private LayerMask damageLayer;
    [field: SerializeField] public int maxHealth { get; private set; } = 5;
    [field: SerializeField] public int currHealth { get; private set; } = 5;
    
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
        enemy.spriteRenderer.enabled = false;
        enemy.enemyCollider.enabled = false;
        enemy.enemyEvents.onEnemyDies?.Invoke();
        Invoke(nameof(Respawn), 1.0f);
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
        if (((1 << collision.gameObject.layer) & damageLayer) == 0) return;

        Player player = collision.gameObject.GetComponentInParent<Player>();
        if (player == null || !player.attack.isAttacking) return;
        if (lastAttackID == player.attack.currAttackID) return;
        
        lastAttackID = player.attack.currAttackID;
        Damage(player.attack.currDamage);
        enemy.enemyRigidbody.AddForce((transform.position - collision.transform.position).normalized * 10.0f, ForceMode2D.Impulse);
    }
}
