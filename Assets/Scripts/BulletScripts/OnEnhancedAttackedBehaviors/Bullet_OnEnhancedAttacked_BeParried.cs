using UnityEngine;

public class Bullet_OnEnhancedAttacked_BeParried : Bullet_OnEnhancedAttacked_BehaviorBase
{
    protected override void OnEnhancedAttackedBehavior(Player player)
    {
        AudioManager.Instance.PlaySoundOneShot(AudioManager.Instance.soundEffects.enemyParried);
        if (bullet.owner == null || !bullet.owner.TryGetComponent<Enemy>(out Enemy enemy)) return;

        player.playerEvents.onParry?.Invoke();
        enemy.enemyEvents.onParried?.Invoke(player.gameObject);
    }
}
