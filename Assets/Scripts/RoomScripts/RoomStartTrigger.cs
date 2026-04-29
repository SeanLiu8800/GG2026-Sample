using UnityEngine;

public class RoomStartTrigger : RoomComponent
{
    private Collider2D roomStartTrigger;
    private SpriteRenderer triggerSprite;
    private int playerLayer;

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
        
        roomStartTrigger = _trigger;
        triggerSprite = _triggerSprite;

        SetActive(true);

        if ((playerLayer = LayerMask.NameToLayer("Player")) == 0) Debug.LogError("Player Layer NOT FOUND!");
    }
    protected void OnEnable()
    {
        room.roomEvents.roomStarts += RoomStarts;
    }
    protected void OnDisable()
    {
        room.roomEvents.roomStarts -= RoomStarts;
    }

    #region ----- Event Functions -----
    void RoomStarts()
    {
        SetActive(false);
    }
    #endregion

    private void SetActive(bool input)
    {
        roomStartTrigger.enabled = input;
        triggerSprite.enabled = input;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != playerLayer) return;

        Debug.Log("Room Starts!");
        room.roomEvents.roomStarts?.Invoke();
    }
}
