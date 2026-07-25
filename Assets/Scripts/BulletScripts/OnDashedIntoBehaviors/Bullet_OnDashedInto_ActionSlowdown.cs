using UnityEngine;

public class Bullet_OnDashedInto_ActionSlowdown : Bullet_OnDashedInto_BehaviorBase
{
    [Header("Action Slowdown Variables")]
    [SerializeField, Range(0.0f, 1.0f)] private float easeInDuration = 0.1f;
    [SerializeField, Range(0.0f, 1.0f)] private float slowdownDuration = 0.1f;
    [SerializeField, Range(0.0f, 1.0f)] private float easeOutDuration = 0.2f;
    protected override void OnDashedInto(Player player)
    {
        SlowdownManager.Instance.ActionSlowdown(easeInDuration, slowdownDuration, easeOutDuration);
    }
}
