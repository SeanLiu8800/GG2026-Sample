using UnityEngine;

public abstract class RoomComponent : MonoBehaviour
{
    public Room room { get; private set; }
    
    protected virtual void Awake()
    {
        room = GetComponentInParent<Room>();
        if (room == null) Debug.LogError($"{this.name} or it's parents DOES NOT have a Room Component!");
    }
}
