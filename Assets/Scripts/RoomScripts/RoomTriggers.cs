using UnityEngine;

public class RoomTriggers : RoomComponent
{
    [SerializeField] private Collider2D roomStartTrigger;
    [SerializeField] private Collider2D roomEndTrigger;
    [SerializeField] private LayerMask playerLayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & playerLayer) == 0) return;

        room.roomEvents.encounterStarts?.Invoke();
    }
}
