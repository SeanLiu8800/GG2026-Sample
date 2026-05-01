using UnityEngine;
using System.Collections;
public class EnemyHealth : EnemyComponent, IDamageable
{
    [field: Header("Health Variables")]
    [field: SerializeField] public float currHealth { get; private set; } = 5;
    [field: SerializeField] public float maxHealth { get; private set; } = 5;

    [field: Header("Element Variables")]
    [field: SerializeField, ReadOnly] public float fireBuildup { get; private set; } = 0.0f;
    [field: SerializeField] public float fireLimit { get; private set; } = 5.0f;
    [field: SerializeField, ReadOnly] public float iceBuildup { get; private set; } = 0.0f;
    [field: SerializeField] public float iceLimit { get; private set; } = 5.0f;
    [field: SerializeField, ReadOnly] public float shockBuildup { get; private set; } = 0.0f;
    [field: SerializeField] public float shockLimit { get; private set; } = 5.0f;
    public bool isCorroded { get; private set; }

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
    public void BulletHits(BulletScript bullet)
    {
        if (bullet.damage < 0.0f)
        {
            Heal(-bullet.damage, bullet.gameObject);
            return;
        }
        if (!enemy.allowDamage) return;
        if (currHealth <= 0.0f) return;

        bullet.bulletEvents.onDamage?.Invoke(this.gameObject);
        Damage(bullet.damage, bullet.element, bullet.elementBuildup, bullet.gameObject);
    }
    public void Damage(float damage, DamageElement element = DamageElement.None, float elementBuildup = 0.0f, GameObject damager = null)
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
            Damage(-heal, DamageElement.None, 0.0f, healer);
            return;
        }

        float originalHealth = currHealth;
        currHealth = Mathf.Clamp(currHealth + heal, 0.0f, maxHealth);
        if (originalHealth != currHealth) enemy.enemyEvents.onHealthChange?.Invoke();
        enemy.enemyEvents.onHeal?.Invoke();

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
    public void Die()
    {
        currHealth = 0;
        enemy.enemyEvents.onEnemyDies?.Invoke();
        enemy.enemyEvents.enemyDies?.Invoke();
    }
    public IEnumerator Afterburn()
    {
        yield break;
    }
    public void ApplyCorrosion(float corrosionThreshold)
    {

    }
}