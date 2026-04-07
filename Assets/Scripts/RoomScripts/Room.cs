using UnityEngine;

public class Room : MonoBehaviour
{
    public GameObject roomEnemyContainer { get; private set; }
    public RoomEventsStruct roomEvents;

    private void Awake()
    {
        roomEnemyContainer = new GameObject("EnemyContainer");
        roomEnemyContainer.transform.parent = this.transform;
        roomEnemyContainer.isStatic = true;
    }
}
