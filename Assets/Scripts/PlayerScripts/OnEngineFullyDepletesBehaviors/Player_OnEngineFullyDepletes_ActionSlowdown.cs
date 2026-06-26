using UnityEngine;

public class Player_OnEngineFullyDepletes_ActionSlowdown : Player_OnEngineFullyDepletes_BehaviorBase
{
    [Tooltip("Action Slowdown Variables")]
    [SerializeField, Range(0.0f, 1.0f)] private float easeInDuration = 0.1f;
    [SerializeField, Range(0.0f, 1.0f)] private float slowdownDuration = 0.0f;
    [SerializeField, Range(0.0f, 1.0f)] private float easeOutDuration = 0.1f;
    protected override void OnEngineFullyDepletes(GameObject depleter)
    {
        SlowdownManager.Instance.ActionSlowdown(easeInDuration, slowdownDuration, easeOutDuration);
    }
}
