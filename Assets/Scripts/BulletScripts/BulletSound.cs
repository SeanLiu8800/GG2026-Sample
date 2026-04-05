using UnityEngine;

public class BulletSound : BulletComponent
{
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
        if (bullet.bulletSFX == null) return;
        if (bullet.bulletSFX.dashedIntoSound == null) return;
        AudioManager.Instance.PlaySoundOneShot(bullet.bulletSFX.dashedIntoSound);
    }
    void OnEnhancedAttacked(Player player)
    {
        if (bullet.bulletSFX == null) return;
        if (bullet.bulletSFX.enhancedAttackedSound == null) return;
        AudioManager.Instance.PlaySoundOneShot(bullet.bulletSFX.enhancedAttackedSound);
    }
    void Start()
    {
        if (bullet.bulletSFX == null) return;
        if (bullet.bulletSFX.firingSound == null) return;
        AudioManager.Instance.PlaySoundOneShot(bullet.bulletSFX.firingSound);
    }
}
