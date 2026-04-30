using UnityEngine;

public class Enemy_OnDeath_SpawnEnemy : Enemy_OnDeath_BehaviorBase
{
    [SerializeField] private GameObject spawnEnemy;
    protected override void Awake()
    {
        base.Awake();

        if (spawnEnemy == null)
        {
            Debug.LogError($"{name}'s spawnEnemy is NULL! No enemy will spawn! Disabling!");
            this.enabled = false;
        }
        if (spawnEnemy.GetComponent<Enemy>() == null) Debug.LogWarning($"{name}'s spawnEnemy is NOT an Enemy! Will Instantiate anyways");
    }
    protected override void OnDeath()
    {
        if (Instantiate(spawnEnemy).TryGetComponent<Enemy>(out Enemy enemy))
        {
            Debug.Log("Spawned an Enemy!");
        }
    }
}
