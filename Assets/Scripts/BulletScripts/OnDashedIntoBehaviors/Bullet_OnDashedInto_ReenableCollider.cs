using UnityEngine;
using System.Collections;
public class Bullet_OnDashedInto_ReenableCollider : Bullet_OnDashedInto_BehaviorBase
{
    [Header("Reenabling Variables")]
    [SerializeField, Range(0.0f, 0.5f)] private float reenablingDelay = 0.1f;
    protected override void OnDashedInto(Player player)
    {
        bullet.bulletCollider.enabled = false;
        StartCoroutine(ReenableCollider());
    }
    private IEnumerator ReenableCollider()
    {
        if (reenablingDelay > 0.0f) yield return new WaitForSeconds(reenablingDelay);
        bullet.bulletCollider.enabled = true;
    }
}
