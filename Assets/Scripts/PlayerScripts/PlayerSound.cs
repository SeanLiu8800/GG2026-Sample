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
    }
    private void OnDisable()
    {
        player.playerEvents.dashStarts -= DashStarts;
        player.playerEvents.perfectDash -= PerfectDash;
        player.playerEvents.imperfectDash -= ImperfectDash;
    }

    void DashStarts()
    {
        AudioManager.Instance.PlaySoundOneShot(sound.dashStart);
    }
    void PerfectDash()
    {
        AudioManager.Instance.PlaySoundOneShot(sound.dashEndPerfect);
    }
    void ImperfectDash()
    {
        AudioManager.Instance.PlaySoundOneShot(sound.dashEndImperfect);
    }
}
