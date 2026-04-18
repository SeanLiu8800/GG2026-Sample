using UnityEngine;
using System;

public struct BulletEvents
{
    public Action<Vector3> onHitWall;

    public Action<Player> onDashedInto;
    public Action<Player> onEnhancedAttacked;

    public Action<Player> onDamagePlayer;
}
