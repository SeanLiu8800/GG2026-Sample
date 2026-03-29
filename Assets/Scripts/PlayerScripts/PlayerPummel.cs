using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerPummel : PlayerComponent
{
    private InputAction moveAction;
    [SerializeField, ReadOnly] private GameObject pummelTarget;
    [SerializeField, ReadOnly] private Vector2 movementInput = Vector2.up;
    [SerializeField, ReadOnly] private Vector3 latchPosition;

    [field : Header("Pummel Variables")]
    [field : SerializeField, ReadOnly] public bool isPummeling { get; private set; } = false;
    protected override void Awake()
    {
        base.Awake();

        moveAction = InputSystem.actions.FindAction("Move");
    }
    private void OnEnable()
    {
        player.playerEvents.pummelStarts += PummelStarts;
        player.playerEvents.pummelEnds += PummelEnds;
        player.playerEvents.pummelReleased += PummelReleased;
        player.playerEvents.pummelEjected += PummelEjected;
    }
    private void OnDisable()
    {
        player.playerEvents.pummelStarts -= PummelStarts;
        player.playerEvents.pummelEnds -= PummelEnds;
        player.playerEvents.pummelReleased -= PummelReleased;
        player.playerEvents.pummelEjected -= PummelEjected;
    }
    #region ----- Event Functions -----
    void PummelStarts(GameObject pummelTarget)
    {
        isPummeling = true;
        this.pummelTarget = pummelTarget;
        // Set Player's position to the closest Enemy Latch Point
        if (pummelTarget.TryGetComponent<EnemyPummel>(out EnemyPummel pummel))
        {
            if (transform.position.x < pummelTarget.transform.position.x) 
                latchPosition = pummel.GetLeftLatchPointPosition();
            else latchPosition = pummel.GetRightLatchPointPosition();
        }
        player.playerCollider.enabled = false;
    }
    void PummelEnds()
    {
        isPummeling = false;
        this.pummelTarget = null;
        player.playerCollider.enabled = true;
    }
    void PummelReleased()
    {

    }
    void PummelEjected()
    {

    }
    #endregion
    void Update()
    {
        movementInput = moveAction.ReadValue<Vector2>();
        if (Keyboard.current.spaceKey.wasPressedThisFrame) DecideAction();
    }
    void FixedUpdate()
    {
        MoveToLatchPosition();
    }

    private void DecideAction()
    {
        if (!isPummeling || pummelTarget == null) return;
        Vector2 toTarget = pummelTarget.transform.position - transform.position;
        if (Vector2.Dot(toTarget.normalized, movementInput.normalized) < -0.8)
        {
            ReleaseTarget();
        }
        else
        {
            Pummel();
        }
    }    
    private void Pummel()
    {
        if (!pummelTarget.TryGetComponent<IDamageable>(out IDamageable damage))
        {
            Debug.LogError($"{pummelTarget.name} DOES NOT have an IDamageable Component!");
        }
        else
        {
            damage.Damage(1);
        }
    }
    private void ReleaseTarget()
    {
        pummelTarget.GetComponent<Enemy>().enemyEvents.pummelEnds?.Invoke();
        player.playerEvents.pummelEnds?.Invoke();
        player.playerEvents.pummelReleased?.Invoke();
    }

    private void MoveToLatchPosition()
    {
        if (!isPummeling) return;
        transform.position = Vector3.Lerp(transform.position, latchPosition, 10.0f * Time.fixedDeltaTime);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!player.movement.isDashing) return;
        if (!collision.TryGetComponent<Enemy>(out Enemy enemy)) return;
        if (!enemy.isParryStunned || enemy.isBeingPummeled) return;

        player.playerEvents.pummelStarts(enemy.gameObject);
        enemy.enemyEvents.pummelStarts(this.gameObject);
    }
}
