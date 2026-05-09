using UnityEngine;

public class EnemySound : EnemyComponent
{
    [SerializeField] private EnemySFX sound;

    protected override void Awake()
    {
        base.Awake();
        if (sound == null)
        {
            Debug.LogError($"{this.name}'s sound is NOT SET! Disabling Component!");
            enabled = false;
        }
    }
    private void OnEnable()
    {
        enemy.enemyEvents.onDamage += OnDamage;
        enemy.enemyEvents.parryStunStarts += ParryStunStarts;
    }
    private void OnDisable()
    {
        enemy.enemyEvents.onDamage -= OnDamage;
        enemy.enemyEvents.parryStunStarts -= ParryStunStarts;
    }

    void OnDamage()
    {
        if (sound.takeDamage != null) AudioManager.Instance.PlaySoundOneShot(sound.takeDamage);
    }
    void ParryStunStarts()
    {
        if (sound.parryStunned != null) AudioManager.Instance.PlaySoundOneShot(sound.parryStunned);
    }
}
