using UnityEngine;
using System;

public struct EnemyEvents
{
    public Action<GameObject> onParried;
    public Action parryStunStarts;
    public Action parryStunEnds;

    public Action onHealthChange;
    public Action onEnemyDies;

    public Action<GameObject> pummelStarts;
    public Action pummelEnds;
}
