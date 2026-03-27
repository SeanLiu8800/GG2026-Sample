using UnityEngine;

public class Player : MonoBehaviour
{
    [field : SerializeField] public PlayerMovement movement { get; private set; }
    [field : SerializeField] public PlayerAttack attack { get; private set; }
    [field : SerializeField] public PlayerHealth health { get; private set; }
    [field: SerializeField] public Collider2D playerCollider { get; private set; }
    [field : SerializeField] public SpriteRenderer spriteRenderer { get; private set; }

    [Header("Ability Toggles")]
    public bool canDash = true;
    public bool canAttack = true;
    public bool canLunge = true;

    public PlayerEvents playerEvents;
}
