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

    [field: SerializeField] public GameObject target { get; private set; }
    public Vector3 toTargetVector 
    { 
        get
        {
            if (target == null) return Vector3.zero;
            return target.transform.position - transform.position;
        }
    }
    public Vector3 toTargetDirection
    {
        get
        {
            return toTargetVector.normalized;
        }
    }
    public float distanceToTarget 
    { 
        get 
        {
            if (target == null) return 0;
            return Vector3.Magnitude(target.transform.position - transform.position);
        }
    }
    public float distanceToTargetSquared
    {
        get
        {
            if (target == null) return 0;
            return Vector3.SqrMagnitude(target.transform.position - transform.position);
        }
    }
    
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

    public bool IsTargetWithinDistance(float distance)
    {
        if (target == null) return false;
        if (distanceToTargetSquared < distance * distance) return true;
        return false;
    }
    public void AssignTarget(GameObject input)
    {
        if (input == null) return;
        // Get Root GameObject of the hierarchy
        while (input.transform.parent != null) input = input.transform.parent.gameObject;
        target = input;
    }
    public void UnassignTarget()
    {
        target = null;
    }
}
