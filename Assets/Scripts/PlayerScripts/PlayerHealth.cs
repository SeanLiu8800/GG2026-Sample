using UnityEngine;

public class PlayerHealth : PlayerComponent, IDamageable
{
    [field: Header("Health Variables")]
    [field: SerializeField] public int maxHealth { get; private set; } = 5;
    [field: SerializeField] public int currHealth { get; private set; } = 5;
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
    
    public void Damage(int damage = 1)
    {
        if (!player.canTakeDamage) return;
        if (isInvincible) return;
        if (damage < 0)
        {
            Heal(-damage);
            return;
        }

        currHealth = Mathf.Clamp(currHealth - damage, 0, maxHealth);
        AudioManager.Instance.PlaySoundOneShot(AudioManager.Instance.soundEffects.playerHurts);
        player.playerEvents.healthChanges?.Invoke();
        if (currHealth <= 0) Die();

        StartInvincibility();
        return;
    }
    public void Heal(int heal = 1)
    {
        if (!player.canHeal) return;
        if (heal < 0)
        {
            Damage(-heal);
            return;
        }

        currHealth = Mathf.Clamp(currHealth + heal, 0, maxHealth);
        player.playerEvents.healthChanges?.Invoke();

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