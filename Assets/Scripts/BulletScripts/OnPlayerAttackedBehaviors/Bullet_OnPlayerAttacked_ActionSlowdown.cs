using UnityEngine;

public class Bullet_OnPlayerAttacked_ActionSlowdown : Bullet_OnPlayerAttacked_BehaviorBase
{
    [SerializeField, Range(0.0f, 1.0f)] private float easeInDuration = 0.1f;
    [SerializeField, Range(0.0f, 1.0f)] private float slowdownDuration = 0.1f;
    [SerializeField, Range(0.0f, 1.0f)] private float easeOutDuration = 0.2f;
    protected override void OnPlayerAttackedBehavior(Player player)
    {
        SlowdownManager.Instance.ActionSlowdown(easeInDuration, slowdownDuration, easeOutDuration);
    }
}
