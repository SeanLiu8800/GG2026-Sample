using UnityEngine;
using System.Collections;
public class Enemy_OnDeath_Respawn : Enemy_OnDeath_BehaviorBase
{
    [SerializeField, Range(0.0f, 5.0f)] private float respawnDelay = 1.0f;
    [SerializeField] private float respawnHealth = 5.0f;
    protected override void OnDeath()
    {
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnDelay);

        enemy.health.Heal(respawnHealth);
        enemy.enemyEvents.enemyRevives?.Invoke();
    }
}
