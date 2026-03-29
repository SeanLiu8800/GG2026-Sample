using UnityEngine;
using System;

public struct EnemyEvents
{
    public Action onHealthChange;
    public Action onEnemyDies;

    public Action<GameObject> onParried;
    public Action onParryStun;
}
