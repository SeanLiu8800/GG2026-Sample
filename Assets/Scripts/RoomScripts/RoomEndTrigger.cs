using UnityEngine;

public class RoomEndTrigger : RoomComponent
{
    [SerializeField] private Collider2D roomEndTrigger;
    [SerializeField] private SpriteRenderer triggerSprite;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private GameObject wallGameObject;
    private Enemy wallEnemy;

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
    }
    protected void OnDisable()
    {
        room.roomEvents.allWavesCompleted -= AllWavesCompleted;
    }
    private void Start()
    {
        wallGameObject =
            Instantiate(
                wallGameObject,
                room.spawnPoints.GetDoorSpawn().transform.position,
                room.spawnPoints.GetDoorSpawn().transform.rotation
            );
        wallGameObject.transform.parent = this.transform;

        if (!wallGameObject.TryGetComponent<Enemy>(out wallEnemy))
        {
            Debug.LogError("Door GameObject DOES NOT have an Enemy Component!");
        }
        else
        {
            wallEnemy.allowInstantPummel = false;
        }
    }
    #region ----- Event Functions -----
    void AllWavesCompleted()
    {
        SetActive(true);
        wallEnemy.allowInstantPummel = true;
        // Allow player to interact with it
        wallEnemy.enemyCollider.layerOverridePriority = 0;
    }
    #endregion

    private void SetActive(bool input)
    {
        roomEndTrigger.enabled = input;
        triggerSprite.enabled = input;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & playerLayer) == 0) return;

        Debug.Log("Room Ends!");
        room.roomEvents.roomEnds?.Invoke();
    }
}
