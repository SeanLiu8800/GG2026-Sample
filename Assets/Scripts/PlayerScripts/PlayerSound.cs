using UnityEngine;

public class PlayerSound : PlayerComponent
{
    [SerializeField] private PlayerSFX sound;
    protected override void Awake()
    {
        base.Awake();
        if (sound == null)
        {
            Debug.LogError("Player's sound Scriptable Object is NOT SET! Disabling Component!");
            enabled = false;
        }
    }
    private void OnEnable()
    {
        player.playerEvents.dashStarts += DashStarts;
        player.playerEvents.perfectDash += PerfectDash;
        player.playerEvents.imperfectDash += ImperfectDash;
        player.playerEvents.enhanceAttack += EnhanceAttack;
        player.playerEvents.onDamage += OnDamage;
    }
    private void OnDisable()
    {
        player.playerEvents.dashStarts -= DashStarts;
        player.playerEvents.perfectDash -= PerfectDash;
        player.playerEvents.imperfectDash -= ImperfectDash;
        player.playerEvents.enhanceAttack -= EnhanceAttack;
        player.playerEvents.onDamage -= OnDamage;
    }

    void DashStarts()
    {
        if (sound.dashStart != null) AudioManager.Instance.PlaySoundOneShot(sound.dashStart);
    }
    void PerfectDash()
    {
        if (sound.dashEndPerfect != null) AudioManager.Instance.PlaySoundOneShot(sound.dashEndPerfect);
    }
    void ImperfectDash()
    {
        if (sound.dashEndImperfect != null) AudioManager.Instance.PlaySoundOneShot(sound.dashEndImperfect);
    }
    void EnhanceAttack()
    {
        if (sound.enhanceAttack != null) AudioManager.Instance.PlaySoundOneShot(sound.enhanceAttack);
    }
    void OnDamage()
    {
        if (sound.takeDamage != null) AudioManager.Instance.PlaySoundOneShot(sound.takeDamage);
    }
}
