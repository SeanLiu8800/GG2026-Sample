using UnityEngine;

public abstract class RoomComponent : MonoBehaviour
{
    public Room room { get; private set; }
    
    protected virtual void Awake()
    {
        if (!TryGetComponent<Room>(out Room _room)) Debug.LogError($"{this.name} DOES NOT have a Room Component!");
        room = _room;
    }
}
