using UnityEngine;

public class RoomEndTrigger : RoomComponent
{
    private Collider2D roomEndTrigger;
    private SpriteRenderer triggerSprite;
    private int playerLayer;
    [SerializeField] private GameObject exitDoorGameObject;
    private Enemy exitDoor;

    protected override void Awake()
    {
        base.Awake();
        if (!TryGetComponent<Collider2D>(out Collider2D _trigger))
        {
            Debug.LogError($"{this.name} DOES NOT have a Collider2D component!");
        }
        if (!TryGetComponent<SpriteRenderer>(out SpriteRenderer _triggerSprite))
        {
            Debug.LogError($"{this.name} DOES NOT have a SpriteRenderer component!");
        }

        roomEndTrigger = _trigger;
        triggerSprite = _triggerSprite;

        SetActive(false);

        if ((playerLayer = LayerMask.NameToLayer("Player")) == 0) Debug.LogError("Player Layer NOT FOUND!");
    }
    protected void OnEnable()
    {
        room.roomEvents.allWavesCompleted += AllWavesCompleted;
        room.roomEvents.roomStarts += RoomStarts;
    }
    protected void OnDisable()
    {
        room.roomEvents.allWavesCompleted -= AllWavesCompleted;
        room.roomEvents.roomStarts -= RoomStarts;
    }
    
    #region ----- Event Functions -----
    void RoomStarts()
    {
        SetActive(false);
        DisableBreakability();
    }
    void AllWavesCompleted()
    {
        SetActive(true);
        EnableBreakability();
    }
    #endregion

    private void Start()
    {
        exitDoorGameObject =
            Instantiate(
                exitDoorGameObject,
                room.spawnPoints.GetDoorSpawn().transform.position,
                room.spawnPoints.GetDoorSpawn().transform.rotation
            );
        exitDoorGameObject.transform.parent = this.transform;

        if (!exitDoorGameObject.TryGetComponent<Enemy>(out exitDoor))
        {
            Debug.LogError("Door GameObject DOES NOT have an Enemy Component!");
        }
        else
        {
            DisableBreakability();
        }
    }
 
    private void SetActive(bool input)
    {
        roomEndTrigger.enabled = input;
        triggerSprite.enabled = input;
    }
    /// <summary>
    /// Function that enables player interaction with enemyWall
    /// </summary>
    public void EnableBreakability()
    {
        if (exitDoor == null)
        {
            Debug.LogError($"{this.name}'s breakable wall is NULL!");
            return;
        }
        exitDoor.allowInstantPummel = true;
        exitDoor.allowDamage = true;
    }
    /// <summary>
    /// Function that disables player interaction with Wall
    /// </summary>
    public void DisableBreakability()
    {
        if (exitDoor == null)
        {
            Debug.LogError($"{this.name}'s breakable wall is NULL!");
            return;
        }
        exitDoor.allowInstantPummel = false;
        exitDoor.allowDamage = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != playerLayer) return;

        Debug.Log("Room Ends!");
        room.roomEvents.roomEnds?.Invoke();
    }
}
