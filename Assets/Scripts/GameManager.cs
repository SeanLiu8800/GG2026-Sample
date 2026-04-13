using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Room currRoom { get; private set; }
    [field: SerializeField] public int roomNumber { get; private set; } = 0;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Another GameManager instance tried to Instantiate! Deleting!");
            Destroy(this.gameObject);
        }
        else
        {
            GameManager.Instance = this;
        }
    }

    private void Start()
    {
        Fun();
    }
    private void Fun()
    {
        currRoom.roomEvents.roomStarts += RoomStarts;
        currRoom.roomEvents.roomEnds += RoomEnd;
    }
    private void RoomStarts()
    {
        Debug.Log("Game Manager sees that Room Starts");
    }
    private void RoomEnd()
    {
        Debug.Log("Game Manager sees that Room Ends");
        roomNumber++;
    }
}
