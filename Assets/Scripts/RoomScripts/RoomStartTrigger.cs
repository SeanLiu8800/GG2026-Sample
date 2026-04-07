using UnityEngine;

public class RoomStartTrigger : RoomComponent
{
    [SerializeField] private Collider2D roomStartTrigger;
    [SerializeField] private LayerMask playerLayer;

    protected override void Awake()
    {
        base.Awake();
        if (!TryGetComponent<Collider2D>(out Collider2D _trigger))
        {
            Debug.LogError($"{this.name} DOES NOT have a Collider2D component!");
        }
        roomStartTrigger = _trigger;

        this.gameObject.SetActive(true);
    }
    protected void OnEnable()
    {
        room.roomEvents.roomStarts += RoomStarts;
    }
    protected void OnDisable()
    {
        room.roomEvents.roomStarts = RoomStarts;
    }

    #region ----- Event Functions -----
    void RoomStarts()
    {
        this.gameObject.SetActive(false);
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & playerLayer) == 0) return;

        Debug.Log("Room Starts!");
        room.roomEvents.roomStarts?.Invoke();
    }
}
