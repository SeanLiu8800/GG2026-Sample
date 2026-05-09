using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region ----- Core Enemy Components -----
    public EnemyMovement move { get; private set; }
    public EnemyAttackBase attack { get; private set; }
    public EnemyHealth health { get; private set; }
    public EnemyVision vision { get; private set; }
    public EnemyParry parry { get; private set; }
    public EnemyPummel pummel { get; private set; }
    public EnemySound sound { get; private set; }
    public SpriteRenderer spriteRenderer { get; private set; }
    public Collider2D enemyCollider { get; private set; }
    public Rigidbody2D enemyRigidbody { get; private set; }
    public EnemyEvents enemyEvents;
    #endregion
    [field: SerializeField] public GameObject target { get; private set; }
    public int playerLayer { get; private set; }
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
            if (target == null) return 0.0f;
            return Vector3.Magnitude(target.transform.position - transform.position);
        }
    }
    public float distanceToTargetSquared
    {
        get
        {
            if (target == null) return 0.0f;
            return Vector3.SqrMagnitude(target.transform.position - transform.position);
        }
    }
    
    [field: Header("Toggles")]
    [field: SerializeField] public bool allowMove { get; private set; } = true;
    [field: SerializeField] public bool allowAttack { get; private set; } = true;
    [field: SerializeField] public bool allowDamage { get; set; } = true;
    [field: SerializeField] public bool allowVision { get; private set; } = true;
    [field: SerializeField] public bool allowInstantPummel { get; set; } = false;

    [Header("States")]
    [ReadOnly] public bool canMove = true;
    [ReadOnly] public bool isAttacking = false;
    [ReadOnly] public bool isParryStunned = false;
    [ReadOnly] public bool pummelOnCooldown = false;
    [ReadOnly] public bool isBeingPummeled = false;
    
    public bool IsPummelable
    {
        get
        {
            if (pummelOnCooldown) return false;
            if (isBeingPummeled) return false;
            if (health.currHealth <= 0) return false;
            if (allowInstantPummel || isParryStunned) return true;
            return false;
        }
    }
    
    void Awake()
    {
        if (!TryGetComponent<EnemyMovement>(out EnemyMovement _move)) Debug.LogError($"{this.name} DOES NOT have an EnemyHealth Component!");
        if (!TryGetComponent<EnemyAttackBase>(out EnemyAttackBase _attack)) Debug.LogError($"{this.name} DOES NOT have an EnemyAttack Component!");
        if (!TryGetComponent<EnemyHealth>(out EnemyHealth _health)) Debug.LogError($"{this.name} DOES NOT have an EnemyHealth Component!");
        if (!TryGetComponent<EnemyVision>(out EnemyVision _vision)) Debug.LogError($"{this.name} DOES NOT have an EnemyVision Component!");
        if (!TryGetComponent<EnemyParry>(out EnemyParry _parry)) Debug.LogError($"{this.name} DOES NOT have an EnemyParry Component!");
        if (!TryGetComponent<EnemyPummel>(out EnemyPummel _pummel)) Debug.LogError($"{this.name} DOES NOT have an EnemyPummel Component!");
        if (!TryGetComponent<EnemySound>(out EnemySound _sound)) Debug.LogError($"{this.name} DOES NOT have an EnemySound Component!");
        if (!TryGetComponent<SpriteRenderer>(out SpriteRenderer _spriteRenderer)) Debug.LogError($"{this.name} DOES NOT have an SpriteRenderer Component!");
        if (!TryGetComponent<Collider2D>(out Collider2D _collider2D)) Debug.LogError($"{this.name} DOES NOT have a Collider2D Component!");
        if (!TryGetComponent<Rigidbody2D>(out Rigidbody2D _rigidbody)) Debug.LogError($"{this.name} DOES NOT have a Rigidbody2D Component!");
        
        move = _move;
        attack = _attack;
        health = _health;
        vision = _vision;
        parry = _parry;
        pummel = _pummel;
        sound = _sound;
        spriteRenderer = _spriteRenderer;
        enemyCollider = _collider2D;
        enemyRigidbody = _rigidbody;
    }
    void OnEnable()
    {
        enemyEvents.onEnemyDies += OnEnemyDies;
    }
    void OnDisable()
    {
        enemyEvents.onEnemyDies -= OnEnemyDies;
    }

    #region ----- Event Function -----
    void OnEnemyDies()
    {
        allowMove = false;
        allowAttack = false;
        allowVision = false;
    }
    #endregion

    void Start()
    {
        // Late addition to current room's enemy spawner
        if (transform.parent == null) GameManager.Instance.currRoom.waveSpawner.AddEnemy(this);
        if ((playerLayer = LayerMask.NameToLayer("Player")) == 0) Debug.LogError("Player layer NOT FOUND!");
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
        // Get Player GameObject of the hierarchy if input is from the Player layer
        if (input.layer == playerLayer) target = input.GetComponentInParent<Player>().gameObject;
        else target = input;
    }
    public void UnassignTarget()
    {
        target = null;
    }
}