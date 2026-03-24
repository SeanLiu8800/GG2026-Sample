using UnityEngine;

public abstract class PlayerComponent : MonoBehaviour
{
    public Player player { get; protected set; }
    protected virtual void Awake()
    {
        if (!TryGetComponent<Player>(out Player _player))
        {
            Debug.LogError($"{this.name} DOES NOT have a Player component!");
            return;
        }

        player = _player;
    }    
}
