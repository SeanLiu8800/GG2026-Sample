using UnityEngine;

public class RoomDebugMenu : RoomComponent
{
    private void Start()
    {
        
    }
    public void KillAllEnemies()
    {
        room.waveSpawner.KillCurrentEnemies();
    }
}
