using UnityEngine;
using System.Collections.Generic;
public class WaveSpawner : RoomComponent
{
    [SerializeField] private EnemyWave[] enemyWaves;
    [SerializeField, ReadOnly] int currWaveNumber = 0;

    [SerializeField, ReadOnly] private List<Enemy> currWaveEnemies;
    [SerializeField, ReadOnly] private int defeatedEnemies = 0;
    private void OnEnable()
    {
        room.roomEvents.waveCompleted += WaveCompleted;
        room.roomEvents.allWavesCompleted += AllWavesCompleted;
    }
    private void OnDisable()
    {
        room.roomEvents.waveCompleted -= WaveCompleted;
        room.roomEvents.allWavesCompleted -= AllWavesCompleted;
    }

    #region ----- Event Functions -----
    void WaveCompleted()
    {
        if (++currWaveNumber >= enemyWaves.Length) room.roomEvents.allWavesCompleted?.Invoke();
        else SpawnWave();
    }
    void AllWavesCompleted()
    {
        currWaveEnemies.Clear();
        Debug.Log("ALL WAVES COMPLETED");
    }
    void OnEnemyDies()
    {
        if (++defeatedEnemies >= currWaveEnemies.Count) room.roomEvents.waveCompleted?.Invoke();
    }
    #endregion
    
    private void Start()
    {
        if (enemyWaves.Length == 0)
        {
            room.roomEvents.allWavesCompleted?.Invoke();
            return;
        }
        currWaveEnemies = new List<Enemy>();
        SpawnWave();
    }
    private void SpawnWave()
    {
        if (currWaveNumber >= enemyWaves.Length)
        {
            Debug.LogError("No more Enemy Waves to Spawn!");
            return;
        }
        defeatedEnemies = 0;
        currWaveEnemies.Clear();
        EnemyWave currWave = enemyWaves[currWaveNumber];
        foreach (EnemyWaveUnit enemyUnit in currWave.enemies)
        {
            if (enemyUnit.enemy == null)
            {
                Debug.LogError($"{this.name} has an Unset enemy unit!");
                continue;
            }
            for (int i = 0; i < enemyUnit.enemyCount; i ++)
            {
                GameObject currEnemyGameObject = Instantiate(enemyUnit.enemy, new Vector2(4.5f, 0.0f), transform.rotation);
                if (!currEnemyGameObject.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    Debug.LogError("Spawned Enemy DOES NOT have an Enemy Component!");
                    Destroy(currEnemyGameObject);
                }
                else
                {
                    currEnemyGameObject.transform.parent = room.roomEnemyContainer.transform;
                    currWaveEnemies.Add(enemy);
                }
            }
        }
        // Track when each enemy Dies
        foreach (Enemy enemy in currWaveEnemies) enemy.enemyEvents.onEnemyDies += OnEnemyDies;
    }
}
[System.Serializable]
public struct EnemyWave
{
    public EnemyWaveUnit[] enemies;
}
[System.Serializable]
public struct EnemyWaveUnit
{
    public GameObject enemy;
    [Range(1, 10)] public int enemyCount;
}
