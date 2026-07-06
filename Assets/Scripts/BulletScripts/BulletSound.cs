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
        bullet.bulletEvents.onPlayerAttacked += OnPlayerAttacked;
    }
    private void OnDisable()
    {
        bullet.bulletEvents.onDashedInto -= OnDashedInto;
        bullet.bulletEvents.onPlayerAttacked -= OnPlayerAttacked;
    }
    void OnDashedInto(Player player)
    {
        if (bulletSFX.dashedIntoSound != null) AudioManager.Instance.PlaySoundOneShot(bulletSFX.dashedIntoSound, bulletSFX.dashedIntoSoundVolume);
    }
    void OnPlayerAttacked(Player player)
    {
        if (bulletSFX.playerAttackedSound != null) AudioManager.Instance.PlaySoundOneShot(bulletSFX.playerAttackedSound, bulletSFX.playerAttackedSoundVolume);
    }
    void Start()
    {
        if (bulletSFX.firingSound != null) AudioManager.Instance.PlaySoundOneShot(bulletSFX.firingSound, bulletSFX.firingSoundVolume);
    }
}
