using UnityEngine;
using System;

public struct PlayerEvents
{
    public Action dashStarts;
    public Action enhanceAttack;
    public Action dashEnds;
    public Action perfectDash;
    public Action imperfectDash;
    public Action dashCooldownEnds;

    public Action<Enemy> pummelStarts;
    public Action pummelEnds;
    public Action pummelReleased; // May be Redudant
    public Action pummelDismount;
    public Action pummelEjected; // May be Redudant

    public Action lungeStarts;
    public Action lungeEnds;

    public Action<Vector3, float> knockbackStarts;
    public Action knockbackEnds;

    public Action attackStarts;
    public Action onParry;
    public Action attackEnds;

    public Action healthChanges; 
    public Action onDamage;
    public Action onHeal;
    public Action playerDies;

    public Action invincibilityStarts;
    public Action invincibilityEnds;
}
