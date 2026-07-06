using UnityEngine;
using System;
public struct BulletEvents
{
    /// <summary>
    /// Vector3 is the normal vector of the wall
    /// </summary>
    public Action<Vector3> onHitWall;   

    public Action<Player> onDashedInto;
    public Action<BulletScript> onPlayerAttackedBullet;
    public Action<Player> onPlayerAttacked;

    public Action<GameObject> onDamage;
}
