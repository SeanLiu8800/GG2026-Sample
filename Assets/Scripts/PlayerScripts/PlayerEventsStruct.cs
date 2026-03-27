using System;

public struct PlayerEvents
{
    public Action dashStarts;
    public Action enhanceAttack;
    public Action dashEnds;
    public Action perfectDash;
    public Action imperfectDash;

    public Action lungeStarts;
    public Action lungeEnds;

    public Action attackStarts;
    public Action attackEnds;

    public Action playerParries;

    public Action healthChanges;
    public Action playerDies;

    public Action invincibilityStarts;
    public Action invincibilityEnds;
}
