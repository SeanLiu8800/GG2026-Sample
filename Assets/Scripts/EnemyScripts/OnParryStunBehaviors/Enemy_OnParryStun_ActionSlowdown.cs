using UnityEngine;

public class Enemy_OnParryStun_ActionSlowdown : Enemy_OnParryStun_BehaviorBase
{
    [SerializeField, Range(0.0f, 2.0f)] private float easeIn = 0.05f;
    [SerializeField, Range(0.0f, 2.0f)] private float slowdownDuration = 0.15f;
    [SerializeField, Range(0.0f, 2.0f)] private float easeOut = 0.2f;
    protected override void OnParryStunBehavior()
    {
        SlowdownManager.Instance.ActionSlowdown(easeIn, slowdownDuration, easeOut);
    }
}
