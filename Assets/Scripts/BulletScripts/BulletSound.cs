using UnityEngine;

public class BulletSound : BulletComponent
{
    [SerializeField] private BulletSFX bulletSFX;
    protected override void Awake()
    {
        base.Awake();
        if (bulletSFX == null)
        {
            Debug.LogError("BulletSFX is NOT SET! Disabling Component!");
            enabled = false;
        }
    }
    private void OnEnable()
    {
        bullet.bulletEvents.onDashedInto += OnDashedInto;
        bullet.bulletEvents.onEnhancedAttacked += OnEnhancedAttacked;
    }
    private void OnDisable()
    {
        bullet.bulletEvents.onDashedInto -= OnDashedInto;
        bullet.bulletEvents.onEnhancedAttacked -= OnEnhancedAttacked;
    }
    void OnDashedInto(Player player)
    {
        if (bulletSFX.dashedIntoSound != null) AudioManager.Instance.PlaySoundOneShot(bulletSFX.dashedIntoSound, bulletSFX.dashedIntoSoundVolume);
    }
    void OnEnhancedAttacked(Player player)
    {
        if (bulletSFX.enhancedAttackedSound != null) AudioManager.Instance.PlaySoundOneShot(bulletSFX.enhancedAttackedSound, bulletSFX.enhancedAttackedSoundVolume);
    }
    void Start()
    {
        if (bulletSFX.firingSound != null) AudioManager.Instance.PlaySoundOneShot(bulletSFX.firingSound, bulletSFX.firingSoundVolume);
    }
}
