using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerPummel : PlayerComponent
{
    private InputAction moveAction;
    [SerializeField, ReadOnly] private GameObject pummelTarget;
    public Vector2 movementInput = Vector2.up;

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
        if (pummelTarget.TryGetComponent<EnemyPummelable>(out EnemyPummelable pummel))
        {
            if (transform.position.x < pummelTarget.transform.position.x) 
                transform.position = pummel.GetLeftLatchPointPosition();
            else transform.position = pummel.GetRightLatchPointPosition();
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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<Enemy>(out Enemy enemy)) return;
        if (!player.movement.isDashing) return;
        player.playerEvents.pummelStarts(enemy.gameObject);
    }
}
