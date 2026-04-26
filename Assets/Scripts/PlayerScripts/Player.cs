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
    public bool allowDash = true;
    public bool allowAttack = true;
    public bool allowLunge = true;
    public bool autoEnhance = false;
    public bool autoLunge = false;

    [Header("Health Toggles")]
    public bool allowDamage = true;
    public bool allowHealing = true;

    [Header("States")]
    [ReadOnly] public bool isDashing = false;
    [ReadOnly] public bool isLunging = false;
    [ReadOnly] public bool isKnockbacked = false;
    [ReadOnly] public bool isAttacking = false;
    [ReadOnly] public bool isPummeling = false;
}
