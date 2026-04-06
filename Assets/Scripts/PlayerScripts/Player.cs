using UnityEngine;

public class Player : MonoBehaviour
{
    [field: SerializeField] public PlayerMovement move { get; private set; }
    [field: SerializeField] public PlayerAttack attack { get; private set; }
    [field: SerializeField] public PlayerHealth health { get; private set; }
    [field: SerializeField] public PlayerPummel pummel { get; private set; }
    [field: SerializeField] public Collider2D playerCollider { get; private set; }
    [field: SerializeField] public SpriteRenderer spriteRenderer { get; private set; }

    public PlayerEvents playerEvents;

    [Header("Movement Ability Toggles")]
    public bool dashIsAvailable = true;
    public bool attackIsAvailable = true;
    public bool lungeIsAvailable = true;
    [Header("Health Toggles")]
    public bool canTakeDamage = true;
    public bool canHeal = true;
    //[Header("States")]
    //[ReadOnly] public bool isPummeling = false;
}
