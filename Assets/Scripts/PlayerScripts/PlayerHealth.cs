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

        player.playerEvents.pummelReleased += PummelRelease;
    }
    void OnDisable()
    {
        player.playerEvents.invincibilityStarts -= InvincibilityStarts;
        player.playerEvents.invincibilityEnds -= InvincibilityEnds;

        player.playerEvents.pummelReleased -= PummelRelease;
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
    void PummelRelease()
    {
        StartInvincibility();
    }
    #endregion
    void Update()
    {
        UpdateInvincibility();
    }
    
    public void Damage(float damage = 1.0f)
    {
        if (!player.canTakeDamage) return;
        if (isInvincible) return;
        if (damage < 0.0f)
        {
            Heal(-damage);
            return;
        }

        float originalHealth = currHealth;
        currHealth = Mathf.Clamp(currHealth - damage, 0.0f, maxHealth);
        if (originalHealth != currHealth) player.playerEvents.healthChanges?.Invoke();
        player.playerEvents.onDamage?.Invoke();

        AudioManager.Instance.PlaySoundOneShot(AudioManager.Instance.soundEffects.playerHurts);
        if (currHealth <= 0.0f) Die();

        StartInvincibility();
        return;
    }
    public void Heal(float heal = 1.0f)
    {
        if (!player.canHeal) return;
        if (heal < 0.0f)
        {
            Damage(-heal);
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