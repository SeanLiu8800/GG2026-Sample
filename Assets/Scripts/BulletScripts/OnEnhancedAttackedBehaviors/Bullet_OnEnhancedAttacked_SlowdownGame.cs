using UnityEngine;

public class Bullet_OnEnhancedAttacked_SlowdownGame : Bullet_OnEnhancedAttacked_BehaviorBase
{
    [SerializeField, Range(0.0f, 1.0f)] private float easeInDuration = 0.01f;
    [SerializeField, Range(0.0f, 1.0f)] private float slowdownDuration = 0.0f;
    [SerializeField, Range(0.0f, 1.0f)] private float easeOutDuration = 0.3f;
    protected override void OnEnhancedAttackedBehavior(Player player)
    {
        SlowdownManager.Instance.ActionSlowdown(easeInDuration, slowdownDuration, easeOutDuration);
    }
}
