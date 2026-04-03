using UnityEngine;

public class Enemy : MonoBehaviour
{
    [field: SerializeField] public EnemyMovement move { get; private set; }
    [field: SerializeField] public EnemyHealth health { get; private set; }
    [field: SerializeField] public EnemyAttack attack { get; private set; }
    [field: SerializeField] public EnemyParry parry { get; private set; }
    [field: SerializeField] public EnemyPummel pummel { get; private set; }
    [field: SerializeField] public SpriteRenderer spriteRenderer { get; private set; }
    [field: SerializeField] public Collider2D enemyCollider { get; private set; }
    [field: SerializeField] public Rigidbody2D enemyRigidbody { get; private set; }

    public EnemyEvents enemyEvents;

    public GameObject target;

    [Header("States")]
    [ReadOnly] public bool canMove = true;
    [ReadOnly] public bool canAttack = false;
    [ReadOnly] public bool isParryStunned = false;
    [ReadOnly] public bool isBeingPummeled = false;

    void Awake()
    {
        if (!TryGetComponent<EnemyHealth>(out EnemyHealth _health))
        {
            Debug.LogError($"{this.name} DOES NOT have an EnemyHealth Component!");
            return;
        }
        health = _health;
        if (!TryGetComponent<SpriteRenderer>(out SpriteRenderer _spriteRenderer))
        {
            Debug.LogError($"{this.name} DOES NOT have an SpriteRenderer Component!");
            return;
        }
        spriteRenderer = _spriteRenderer;
        if (!TryGetComponent<Collider2D>(out Collider2D _collider2D))
        {
            Debug.LogError($"{this.name} DOES NOT have a Collider2D Component!");
            return;
        }
        enemyCollider = _collider2D;
    }
}
