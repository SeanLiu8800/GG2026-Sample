using UnityEngine;

public class RoomEndTrigger : RoomComponent
{
    private Collider2D roomEndTrigger;
    private SpriteRenderer triggerSprite;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GameObject breakableWallGameObject;
    private Enemy breakableWall;

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
        DisableWall();
    }
    void AllWavesCompleted()
    {
        SetActive(true);
        EnableWall();
    }
    #endregion

    private void Start()
    {
        breakableWallGameObject =
            Instantiate(
                breakableWallGameObject,
                room.spawnPoints.GetDoorSpawn().transform.position,
                room.spawnPoints.GetDoorSpawn().transform.rotation
            );
        breakableWallGameObject.transform.parent = this.transform;

        if (!breakableWallGameObject.TryGetComponent<Enemy>(out breakableWall))
        {
            Debug.LogError("Door GameObject DOES NOT have an Enemy Component!");
        }
        else
        {
            DisableWall();
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
    public void EnableWall()
    {
        if (breakableWall == null)
        {
            Debug.LogError($"{this.name}'s breakable wall is NULL!");
            return;
        }
        breakableWall.allowInstantPummel = true;
        breakableWall.allowDamage = true;
        breakableWall.enemyCollider.layerOverridePriority = -1;
    }
    /// <summary>
    /// Function that disables player interaction with Wall
    /// </summary>
    public void DisableWall()
    {
        if (breakableWall == null)
        {
            Debug.LogError($"{this.name}'s breakable wall is NULL!");
            return;
        }
        breakableWall.allowInstantPummel = false;
        breakableWall.allowDamage = false;
        breakableWall.enemyCollider.layerOverridePriority = 1;
        breakableWall.enemyCollider.includeLayers += playerLayer;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & playerLayer) == 0) return;

        Debug.Log("Room Ends!");
        room.roomEvents.roomEnds?.Invoke();
    }
}
