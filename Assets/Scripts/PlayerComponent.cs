using UnityEngine;

public abstract class PlayerComponent : MonoBehaviour
{
    public Player player { get; private set; }
    protected virtual void Awake()
    {
        if (!TryGetComponent<Player>(out Player _player))
            Debug.LogError($"{this.name} DOES NOT have a Player component!");

        player = _player;
    }    
}
