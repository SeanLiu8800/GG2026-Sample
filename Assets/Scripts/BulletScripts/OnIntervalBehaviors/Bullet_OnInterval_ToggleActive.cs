using UnityEngine;
/// <summary>
/// Allows Bullet to Toggle it's Player Damage Activity during an Interval<br/>
/// This also can be used to Telegraph Melee Attacks by...<br/>
/// -Setting startActive to false<br/>
/// -Using initialActionDelay as the Telegraphing Duration<br/>
/// -Setting actionCount to 1< (actionInterval can be anything since actionCount is 1)<br/>
/// </summary>
public class Bullet_OnInterval_ToggleActive : Bullet_OnIntervalBehaviorBase
{
    [SerializeField] private bool startActive = false;
    private bool activity = true;
    protected override void Start()
    {
        base.Start();
        if (bullet.spriteRenderer == null) Debug.LogWarning($"{this.name} DOES NOT have a Sprite Renderer!");
        if (bullet.bulletCollider.enabled != startActive) ToggleActive();
    }
    protected override void IntervalAction()
    {
        ToggleActive();
    }
    
    /// <summary>
    ///     Toggles the Collider of the Bullet<br/>
    ///     Sets the Alpha Value of the Bullet's SpriteRenderer to 0.3f if it's Inactive<br/>
    ///     Sets the Alpha Value of the Bullet's SpriteRenderer to 1.0f if it's Active<br/>
    /// </summary>
    private void ToggleActive()
    {
        bullet.bulletCollider.enabled = (activity = !activity);
        if (bullet.spriteRenderer != null) bullet.spriteRenderer.SetAlpha(activity ? 1.0f : 0.3f);
    }
}
