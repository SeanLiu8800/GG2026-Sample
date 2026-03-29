using System;

public struct PlayerEvents
{
    public Action dashStarts;
    public Action enhanceAttack;
    public Action dashEnds;
    public Action perfectDash;
    public Action imperfectDash;
    public Action dashCooldownEnds;

    public Action lungeStarts;
    public Action lungeEnds;

    public Action attackStarts;
    public Action onParry;
    public Action attackEnds;

    public Action healthChanges;
    public Action playerDies;

    public Action invincibilityStarts;
    public Action invincibilityEnds;
}
