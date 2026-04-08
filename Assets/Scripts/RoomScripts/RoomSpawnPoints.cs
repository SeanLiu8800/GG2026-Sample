using UnityEngine;

public class RoomSpawnPoints : RoomComponent
{
    [SerializeField] private GameObject playerSpawnPoint;
    [SerializeField] private GameObject enemySpawnPointCollection;

    private void Start()
    {
        if (playerSpawnPoint == null) Debug.LogError($"{this.name}'s playerSpawnPoint is NULL! Set something to it!");
        if (enemySpawnPointCollection == null) Debug.LogError($"{this.name}'s enemySpawnPoints is NULL! Set something to it!");
        if (enemySpawnPointCollection.transform.childCount <= 0) Debug.LogError($"{this.name}'s enemySpawnPointCollection is EMPTY! Give it some Values!");
    }
    
    public Vector3 GetPlayerSpawnPoint()
    {
        if (playerSpawnPoint == null)
        {
            Debug.LogWarning($"playerSpawnPoint is NULL! falling back to this room's position!");
            return transform.position;
        }
        return playerSpawnPoint.transform.position;
    }
    private int enemySpawnPointIndex = 0;
    public Vector3 GetEnemySpawnPoint()
    {
        if (enemySpawnPointCollection == null || enemySpawnPointCollection.transform.childCount <= 0)
        {
            Debug.LogWarning($"enemySpawnPointCollection is NULL or EMPTY! falling back to this room's position!");
            return transform.position;
        }
        Vector3 spawnPoint = enemySpawnPointCollection.transform.GetChild(enemySpawnPointIndex).position;
        enemySpawnPointIndex = (enemySpawnPointIndex + 1) % enemySpawnPointCollection.transform.childCount;
        return spawnPoint;
    }
}
