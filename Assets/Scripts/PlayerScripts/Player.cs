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
    [ReadOnly] public PlayerState state = PlayerState.Idle;

    public bool isIdle { get { return state == 0; } }
    public bool isDashing { get { return (state & PlayerState.Dashing) != 0; } }
    public bool isLunging { get { return (state & PlayerState.Lunging) != 0; } }
    public bool isAttacking { get { return (state & PlayerState.Attacking) != 0; } }
    public bool isKnockbacked { get { return (state & PlayerState.Knockbacked) != 0; } }
    public bool isPummeling { get { return (state & PlayerState.Pummeling) != 0; } }
    public bool isRestricted { get { return (state & PlayerState.Restricted) != 0; } }
}

[System.Flags] public enum PlayerState 
{ 
    Idle = 0, 
    Dashing = 1, 
    Lunging = 2, 
    Attacking = 4, 
    Knockbacked = 8, 
    Pummeling = 16,
    Restricted = 32
}
public static class PlayerStateExtensions
{
    public static PlayerState Add(this PlayerState state, PlayerState input)
    {
        return state | input;
    }
    public static PlayerState Remove(this PlayerState state, PlayerState input)
    {
        return state & ~input;
    }
    public static bool IsIdle(this PlayerState state)
    {
        if (state == PlayerState.Idle) return true;
        else return false;
    }
}