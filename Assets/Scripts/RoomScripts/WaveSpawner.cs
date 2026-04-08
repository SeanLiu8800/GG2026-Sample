using UnityEngine;
using System.Collections.Generic;
public class WaveSpawner : RoomComponent
{
    [field: Tooltip("Determines whether to spawn wave 1 immediately, or wait for StartEncounter. \nSets itself to False after spawning immediately.")]
    [field: SerializeField] public bool spawnImmediately { get; private set; } = false;
    [SerializeField] private EnemyWave[] enemyWaves;
    [SerializeField, ReadOnly] int currWaveNumber = 0;

    [HideInInspector] public GameObject enemyContainer { get; private set; }
    [SerializeField, ReadOnly] private List<Enemy> currWaveEnemies;
    [SerializeField, ReadOnly] private int defeatedEnemies = 0;

    public bool noEnemyWaves { get { return enemyWaves.Length <= 0; } }
    protected override void Awake()
    {
        base.Awake();

        enemyContainer = new GameObject("EnemyContainer");
        enemyContainer.transform.parent = this.transform;
        enemyContainer.isStatic = true;
    }
    private void OnEnable()
    {
        room.roomEvents.roomStarts += RoomStarts;
        room.roomEvents.waveCompleted += WaveCompleted;
        room.roomEvents.allWavesCompleted += AllWavesCompleted;
    }
    private void OnDisable()
    {
        room.roomEvents.roomStarts -= RoomStarts;
        room.roomEvents.waveCompleted -= WaveCompleted;
        room.roomEvents.allWavesCompleted -= AllWavesCompleted;
    }

    #region ----- Event Functions -----
    void RoomStarts()
    {
        if (spawnImmediately)
        {
            spawnImmediately = false; // Purely if we want to spawn the same encounter again
            return;
        }

        DeleteEnemies();
        currWaveNumber = 0;
        SpawnWave();
    }
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
        if (spawnImmediately)
        {
            spawnImmediately = false;
            SpawnWave();
        }
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
                    currEnemyGameObject.transform.parent = enemyContainer.transform;
                    currEnemyGameObject.transform.position = room.spawnPoints.GetEnemySpawnPoint();
                    currWaveEnemies.Add(enemy);
                }
            }
        }
        // Track when each enemy Dies
        foreach (Enemy enemy in currWaveEnemies) enemy.enemyEvents.onEnemyDies += OnEnemyDies;
    }
    private void DeleteEnemies()
    {
        foreach (Transform childTransform in enemyContainer.transform) Destroy(childTransform.gameObject);
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
