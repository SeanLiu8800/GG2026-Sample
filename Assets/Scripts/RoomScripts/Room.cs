using UnityEngine;

public class Room : MonoBehaviour
{
    public WaveSpawner waveSpawner { get; private set; }
    public RoomSpawnPoints spawnPoints { get; private set; }
    public RoomCamera roomCamera { get; private set; }

    public RoomEventsStruct roomEvents;

    private void Awake()
    {
        if (!TryGetComponent<WaveSpawner>(out WaveSpawner _waveSpawner))
        {
            Debug.LogError($"{this.name} DOES NOT have a Wave Spawner Component!");
        }
        if (!TryGetComponent<RoomSpawnPoints>(out RoomSpawnPoints _spawnPoints))
        {
            Debug.LogError($"{this.name} DOES NOT have a RoomSpawnPoints Component!");
        }
        if (!TryGetComponent<RoomCamera>(out RoomCamera _roomCamera))
        {
            Debug.LogError($"{this.name} DOES NOT have a RoomCamera Component!");
        }

        waveSpawner = _waveSpawner;
        spawnPoints = _spawnPoints;
        roomCamera = _roomCamera;
    }
}
