using UnityEngine;
using System.Collections.Generic;
public class WaveSpawner : RoomComponent
{
    [field: Tooltip("Determines whether to spawn wave 1 immediately, or wait for StartEncounter")]
    [field: SerializeField] public bool spawnImmediately { get; private set; } = false;
    private bool suppressFirstWave = false;
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
        if (spawnImmediately && suppressFirstWave)
        {
            Debug.Log("Spawn suppressed!");
            suppressFirstWave = false;
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
            suppressFirstWave = true;
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
                    AddEnemy(enemy);
                }
            }
        }
    }
    public void AddEnemy(Enemy enemy)
    {
        enemy.transform.parent = enemyContainer.transform;
        enemy.transform.position = room.spawnPoints.GetEnemySpawnPoint();
        enemy.enemyEvents.onEnemyDies += OnEnemyDies;
        currWaveEnemies.Add(enemy);
    }
    public void DeleteEnemies()
    {
        foreach (Transform childTransform in enemyContainer.transform) Destroy(childTransform.gameObject);
    }
    public void KillCurrentEnemies()
    {
        for (int i = 0; i < currWaveEnemies.Count; i ++) currWaveEnemies[i].enemyEvents.onEnemyDies?.Invoke();
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
