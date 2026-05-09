using UnityEngine;
using System;
using System.Collections;
public class EnemyHealth : EnemyComponent, IDamageable
{
    [field: Header("Health Variables")]
    [field: SerializeField] public float maxHealth { get; private set; } = 5.0f;
    [SerializeField] private float _currHealth = 5.0f;
    public float currHealth { get { return _currHealth; } private set { _currHealth = value; onHealthChange?.Invoke(); } }
    public Action onHealthChange { get; set; }
    
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
        enemy.enemyEvents.onHeal?.Invoke();

        return;
    }
    public void ElementDamage(DamageElement element, float buildupRate)
    {
        if ((element & DamageElement.Fire) != 0)
        {
            fireBuildup += buildupRate;
            if (fireBuildup >= fireLimit)
            {
                Debug.Log("Player suffers Overheat");
                fireBuildup = 0.0f;
            }
        }
        if ((element & DamageElement.Ice) != 0)
        {
            iceBuildup += buildupRate;
            if (iceBuildup >= iceLimit)
            {
                Debug.Log("Player suffers Cold Seizure");
                iceBuildup = 0.0f;
            }
        }
        if ((element & DamageElement.Shock) != 0)
        {
            shockBuildup += buildupRate;
            if (shockBuildup >= shockLimit)
            {
                Debug.Log("Player suffers Undervolt");
                shockBuildup = 0.0f;
            }
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