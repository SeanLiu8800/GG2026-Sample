using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private BiomeComponents biomeComponents;
    [SerializeField] private Player player;
    [field: SerializeField] public Room currRoom { get; private set; }
    [SerializeField, ReadOnly] int[] roomOrder;
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
        StartGame();
    }
    private void StartGame()
    {
        GenerateRoomOrder();
        SpawnRoom();
        
    }
    private void GenerateRoomOrder()
    {
        roomOrder = new int[biomeComponents.biomeLength];
        int roomOrderIndex = 0;
        while (roomOrderIndex < roomOrder.Length)
        {
            int[] currRoomOrder = new int[biomeComponents.biomeCombatRooms.Length];
            for (int i = 0; i < biomeComponents.biomeCombatRooms.Length; i++) currRoomOrder[i] = i;
            ShuffleArray(currRoomOrder);
            for (int i = 0; i < currRoomOrder.Length; i++)
            {
                roomOrder[roomOrderIndex++] = currRoomOrder[i];
                if (roomOrderIndex >= roomOrder.Length) return;
            }
        }
    }
    private void SpawnRoom()
    {
        if (currRoom != null)
        {
            Destroy(currRoom.gameObject);
            Unsubscribe();
        }
        currRoom = Instantiate(biomeComponents.biomeCombatRooms[roomOrder[roomNumber]]).GetComponent<Room>();
        player.transform.position = currRoom.spawnPoints.GetPlayerSpawnPoint();
        Subscribe();
    }

    private void Subscribe()
    {
        currRoom.roomEvents.roomStarts += RoomStarts;
        currRoom.roomEvents.roomEnds += LoadNextRoom;
    }
    private void Unsubscribe()
    {
        currRoom.roomEvents.roomStarts -= RoomStarts;
        currRoom.roomEvents.roomEnds -= LoadNextRoom;
    }
    private void RoomStarts()
    {
        Debug.Log("Game Manager sees that Room Starts");
    }
    private void LoadNextRoom()
    {
        Debug.Log("Game Manager sees that Room Ends");
        roomNumber ++;
        if (roomNumber < roomOrder.Length) SpawnRoom();
        else
        {
            roomNumber = -1;
            Debug.Log("Biome Completed!");
        }
    }
    private void ShuffleArray(int[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int temp = array[i];
            int randomIndex = Random.Range(i, array.Length);
            array[i] = array[randomIndex];
            array[randomIndex] = temp;
        }
    }
}
