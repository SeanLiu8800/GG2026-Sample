using UnityEngine;
using System;

public struct EnemyEvents
{
    public Action meleeAttack;
    public Action rangedAttack;
    public Action attackEnds;

    public Action<GameObject, float> onParried;
    public Action parryProgressChanges;
    public Action parryStunStarts;
    public Action parryStunEnds;

    public Action onDamage;
    public Action onHeal;
    public Action onEnemyDies;
    public Action enemyDies;
    public Action enemyRevives;

    public Action<Player> pummelStarts;
    public Action pummelEnds;
}
