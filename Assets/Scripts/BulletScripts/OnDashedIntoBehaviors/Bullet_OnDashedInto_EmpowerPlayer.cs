using UnityEngine;

public class Bullet_OnDashedInto_EmpowerPlayer : Bullet_OnDashedInto_BehaviorBase
{
    protected override void OnDashedInto(Player player)
    {
        player.attack.Empower(bullet.empowerRate);
    }
}
