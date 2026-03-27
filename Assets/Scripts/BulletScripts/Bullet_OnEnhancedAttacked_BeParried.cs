using UnityEngine;

public class Bullet_OnEnhancedAttacked_BeParried : Bullet_OnEnhancedAttacked_BehaviorBase
{
    protected override void OnEnhancedAttackedBehavior()
    {
        if (bullet.owner == null || !bullet.owner.TryGetComponent<Enemy>(out Enemy enemy)) return;

        AudioManager.Instance.PlaySoundOneShot(AudioManager.Instance.soundEffects.enemyParried);
        enemy.attack.Parried();
    }
}
