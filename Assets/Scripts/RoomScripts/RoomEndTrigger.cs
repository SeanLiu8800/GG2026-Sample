using UnityEngine;

public class RoomEndTrigger : RoomComponent
{
    [SerializeField] private Collider2D roomEndTrigger;
    [SerializeField] private LayerMask playerLayer;

    protected override void Awake()
    {
        base.Awake();
        if (!TryGetComponent<Collider2D>(out Collider2D _trigger))
        {
            Debug.LogError($"{this.name} DOES NOT have a Collider2D component!");
        }

        roomEndTrigger = _trigger;

        this.gameObject.SetActive(false);
    }
    protected void OnEnable()
    {
        room.roomEvents.allWavesCompleted += AllWavesCompleted;
    }
    protected void OnDisable()
    {
        room.roomEvents.allWavesCompleted -= AllWavesCompleted;
    }

    #region ----- Event Functions -----
    void AllWavesCompleted()
    {
        this.gameObject.SetActive(true);
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & playerLayer) == 0) return;

        Debug.Log("Room Ends!");
        room.roomEvents.roomEnds?.Invoke();
    }
}
