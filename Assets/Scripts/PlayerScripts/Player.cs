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
}

[System.Flags]
public enum PlayerState { Idle = 0, Moving = 1, Dashing = 2, Lunging = 4, Attacking = 8, Knockbacked = 16, Pummeling = 32}
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