using UnityEngine;

public class Bullet_OnDamage_ResetPlayerDashCooldown : Bullet_OnDamage_BehaviorBase
{
    private Player playerOwner;
    private void Start()
    {
        if (!bullet.owner.TryGetComponent<Player>(out playerOwner))
        {
            Debug.LogError("This bullet's Owner DOES NOT have a Player Component! Disabling!");
            this.enabled = false;
        }
    }
    protected override void OnDamage(GameObject hitObject)
    {
        playerOwner.playerEvents.dashCooldownEnds?.Invoke();
    }
}
