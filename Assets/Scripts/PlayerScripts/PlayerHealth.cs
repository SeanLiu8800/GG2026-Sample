using UnityEngine;

public class PlayerHealth : PlayerComponent, IDamageable
{
    [field: Header("Health Variables")]
    [field: SerializeField] public float maxHealth { get; private set; } = 5.0f;
    [field: SerializeField] public float currHealth { get; private set; } = 5.0f;
    [field: Header("Invincibility Variables")]
    [field: SerializeField, ReadOnly] public bool isInvincible { get; private set; } = false;
    [SerializeField, Range(0.0f, 2.0f)] private float invincibilityDuration = 1.0f;
    private float invincibilityEndTime = 0.0f;
    void OnEnable()
    {
        player.playerEvents.invincibilityStarts += InvincibilityStarts;
        player.playerEvents.invincibilityEnds += InvincibilityEnds;

        player.playerEvents.pummelDismount += PummelDismount;
    }
    void OnDisable()
    {
        player.playerEvents.invincibilityStarts -= InvincibilityStarts;
        player.playerEvents.invincibilityEnds -= InvincibilityEnds;

        player.playerEvents.pummelDismount -= PummelDismount;
    }

    #region ----- Event Functions -----
    void InvincibilityStarts()
    {
        isInvincible = true;
        player.spriteRenderer.SetAlpha(0.5f);
    }
    void InvincibilityEnds()
    {
        isInvincible = false;
        player.spriteRenderer.SetAlpha(1.0f);
    }
    void PummelDismount()
    {
        StartInvincibility();
    }
    #endregion
    void Update()
    {
        UpdateInvincibility();
    }

    public void Damage(float damage, BulletScript bullet)
    {
        if (damage < 0.0f)
        {
            Heal(-damage, bullet.gameObject);
            return;
        }
        if (player.move.isDashing)
        {
            bullet.bulletEvents.onDashedInto?.Invoke(player);
            return;
        }
        if (!player.allowDamage) return;
        if (player.attack.isAttacking && player.attack.attackIsEnhanced) return; // Yes this is supposed to be blank, because enhanced attack is now a Bullet Component
        if (!isInvincible)
        {
            bullet.bulletEvents.onDamage?.Invoke(this.gameObject);
            Damage(damage, bullet.gameObject);
        }
    }
    public void Damage(float damage, GameObject damager = null)
    {
        if (damage < 0.0f)
        {
            Heal(-damage, damager);
            return;
        }
        if (!player.allowDamage) return;
        if (isInvincible) return;

        float originalHealth = currHealth;
        currHealth = Mathf.Clamp(currHealth - damage, 0.0f, maxHealth);
        if (originalHealth != currHealth) player.playerEvents.healthChanges?.Invoke();
        player.playerEvents.onDamage?.Invoke();

        AudioManager.Instance.PlaySoundOneShot(AudioManager.Instance.soundEffects.playerHurts);
        if (currHealth <= 0.0f) Die();

        StartInvincibility();
        return;
    }
    public void Heal(float heal, GameObject healer = null)
    {
        if (!player.allowHealing) return;
        if (heal < 0.0f)
        {
            Damage(-heal, healer);
            return;
        }

        float originalHealth = currHealth;
        currHealth = Mathf.Clamp(currHealth + heal, 0.0f, maxHealth);
        if (originalHealth != currHealth) player.playerEvents.healthChanges?.Invoke();
        player.playerEvents.onHeal?.Invoke();

        return;
    }
    public void Die()
    {
        Debug.Log($"{this.name} has run out of health!");
    }

    private void StartInvincibility(float duration = -1.0f)
    {
        if (duration < 0.0f) duration = invincibilityDuration;
        // don't update Invincibility Time if current IFrame period will outlast the input duration
        if (Time.time + duration < invincibilityEndTime) return;

        invincibilityEndTime = Time.time + duration;
        player.playerEvents.invincibilityStarts?.Invoke();
    }
    private void UpdateInvincibility()
    {
        if (!isInvincible || Time.time < invincibilityEndTime) return;
        player.playerEvents.invincibilityEnds?.Invoke();
    }
}