using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
public class PlayerHealth : PlayerComponent, IDamageable
{
    [field: Header("Health Variables")]
    [field: SerializeField] public float maxHealth { get; private set; } = 5.0f;
    [field: SerializeField] public float currHealth { get; private set; } = 5.0f;

    [field: Header("Element Variables")]
    [field: SerializeField, ReadOnly] public float fireBuildup { get; private set; } = 0.0f;
    [field: SerializeField] public float fireLimit { get; private set; } = 5.0f;
    [field: SerializeField, ReadOnly] public float iceBuildup { get; private set; } = 0.0f;
    [field: SerializeField] public float iceLimit { get; private set; } = 5.0f;
    [field: SerializeField, ReadOnly] public float shockBuildup { get; private set; } = 0.0f;
    [field: SerializeField] public float shockLimit { get; private set; } = 5.0f;
    [field: SerializeField] public bool isCorroded { get; private set; }

    [field: Header("Invincibility Variables")]
    [field: SerializeField, ReadOnly] public bool isInvincible { get; private set; } = false;
    [SerializeField, Range(0.0f, 2.0f)] private float invincibilityDuration = 1.0f;
    private float invincibilityEndTime = 0.0f;
    void OnEnable()
    {
        player.playerEvents.invincibilityStarts += InvincibilityStarts;
        player.playerEvents.invincibilityEnds += InvincibilityEnds;

        player.playerEvents.pummelDismount += PummelDismount;

        player.playerEvents.onParry += OnParry;
    }
    void OnDisable()
    {
        player.playerEvents.invincibilityStarts -= InvincibilityStarts;
        player.playerEvents.invincibilityEnds -= InvincibilityEnds;

        player.playerEvents.pummelDismount -= PummelDismount;

        player.playerEvents.onParry -= OnParry;
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
    void OnParry()
    {
        CorrosionResetGracePeriod();
    }
    #endregion
    void Update()
    {
        UpdateInvincibility();
        if (Keyboard.current.nKey.wasPressedThisFrame) ApplyCorrosion(0.5f);
        if (Keyboard.current.mKey.wasPressedThisFrame) StopCorrosion();
    }

    public void BulletHits(BulletScript bullet)
    {
        if (bullet.damage < 0.0f)
        {
            Heal(-bullet.damage, bullet.gameObject);
            return;
        }
        if (player.isDashing) return;
        if (!player.allowDamage) return;
        if (player.isAttacking && player.attack.attackIsEnhanced) return; // Yes this is supposed to be blank, because enhanced attack is now a Bullet Component
        if (!isInvincible)
        {
            bullet.bulletEvents.onDamage?.Invoke(this.gameObject);
            Damage(bullet.damage, bullet.element, bullet.elementBuildup, bullet.gameObject);
        }
    }
    public void Damage(float damage, DamageElement element = DamageElement.None, float elementBuildup = 0.0f, GameObject damager = null)
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

        if (element != DamageElement.Corrosion) AudioManager.Instance.PlaySoundOneShot(AudioManager.Instance.soundEffects.playerHurts);
        if (currHealth <= 0.0f) Die();

        if (element != DamageElement.Corrosion) StartInvincibility();
        return;
    }
    public void ElementDamage(DamageElement element, float buildupRate)
    {
        switch (element)
        {
            case DamageElement.Fire:
                fireBuildup += buildupRate;
                break;
            case DamageElement.Ice:
                iceBuildup += buildupRate;
                break;
            case DamageElement.Shock:
                shockBuildup += buildupRate;
                break;
            default:
                break;
        }
    }
    public void Heal(float heal, GameObject healer = null)
    {
        if (!player.allowHealing) return;
        if (heal < 0.0f)
        {
            Damage(-heal, DamageElement.None, 0.0f, healer);
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

    public IEnumerator Afterburn()
    {
        yield break;
    }
    public void ApplyCorrosion(float corrosionThreshold)
    {
        StopCorrosion();
        corrosionCoroutine = StartCoroutine(Corrosion(corrosionThreshold));
    }
    public void StopCorrosion()
    {
        isCorroded = false;
        if (corrosionCoroutine != null) StopCoroutine(corrosionCoroutine);
    }
    public void CorrosionResetGracePeriod()
    {
        currCorrosionGrace = corrosionGracePeriod;
    }
    [SerializeField, Range(0.0f, 10.0f)] private float corrosionGracePeriod = 5.0f;
    [SerializeField, Range(0.0f, 10.0f)] private float currCorrosionGrace = 5.0f;
    private Coroutine corrosionCoroutine;
    private IEnumerator Corrosion(float corrosionThreshold)
    {
        isCorroded = true;
        CorrosionResetGracePeriod();
        while (true)
        {
            if (currCorrosionGrace < 0.0f && currHealth > maxHealth * corrosionThreshold)
                Damage(1.0f * Time.deltaTime, DamageElement.Corrosion);
            yield return null;
            currCorrosionGrace -= Time.deltaTime;
        }
    }
}