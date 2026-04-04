using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class PlayerPummel : PlayerComponent
{
    private InputAction moveAction;
    private InputAction pummelAction;

    [field: Header("Pummel Variables")]
    [field: SerializeField, ReadOnly] public bool isPummeling { get; private set; } = false;
    [SerializeField, ReadOnly] private Enemy pummelTarget;
    [SerializeField, ReadOnly] private int latchPositionIndex = 0;
    
    protected override void Awake()
    {
        base.Awake();

        moveAction = InputSystem.actions.FindAction("Move");
        pummelAction = InputSystem.actions.FindAction("PlayerPummelControls/Pummel");
    }
    private void OnEnable()
    {
        pummelAction.started += DecideAction;

        player.playerEvents.pummelStarts += PummelStarts;
        player.playerEvents.pummelEnds += PummelEnds;
        player.playerEvents.pummelReleased += PummelReleased;
        player.playerEvents.pummelEjected += PummelEjected;
    }
    private void OnDisable()
    {
        pummelAction.started -= DecideAction;

        player.playerEvents.pummelStarts -= PummelStarts;
        player.playerEvents.pummelEnds -= PummelEnds;
        player.playerEvents.pummelReleased -= PummelReleased;
        player.playerEvents.pummelEjected -= PummelEjected;
    }
    #region ----- Event Functions -----
    void PummelStarts(Enemy enemy)
    {
        isPummeling = true;
        pummelTarget = enemy;
        // Set Player's position to the closest Enemy Latch Point
        if (transform.position.x < pummelTarget.transform.position.x) latchPositionIndex = 0;
        else latchPositionIndex = 1;

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
    void FixedUpdate()
    {
        MoveToLatchPosition();
    }

    private void DecideAction(InputAction.CallbackContext context)
    {
        if (!isPummeling || pummelTarget == null) return;
        Vector2 toTarget = pummelTarget.transform.position - transform.position;
        Vector3 directionInput = moveAction.ReadValue<Vector2>();
        if (Vector2.Dot(toTarget.normalized, directionInput.normalized) < -0.6)
        {
            Debug.LogWarning($"Releasing {pummelTarget.name}");
            ReleaseTarget();
        }
        else
        {
            Debug.LogWarning($"Pummeling {pummelTarget.name}");
            Pummel();
        }
    }    
    private void Pummel()
    {
        pummelTarget.health.Damage(1);
        pummelTarget.enemyRigidbody.AddForce(
            (pummelTarget.transform.position - transform.position).normalized * 4.0f, 
            ForceMode2D.Impulse
        );
        if (pummelTarget.health.currHealth <= 0) StartCoroutine(PummelDismount());
    }
    private void ReleaseTarget()
    {
        pummelTarget.enemyEvents.pummelEnds?.Invoke();
        player.playerEvents.pummelEnds?.Invoke();
        player.playerEvents.pummelReleased?.Invoke();
    }

    private IEnumerator PummelDismount()
    {
        pummelTarget.enemyEvents.pummelEnds?.Invoke();
        pummelTarget = null;

        float dismountDuration = 0.5f;
        float dismountStartTime = Time.time;
        Vector3 dismountEndPosition = Vector3.zero;
        while (Time.time - dismountStartTime < dismountDuration)
        {
            transform.position = Vector3.Lerp(transform.position, dismountEndPosition, 5.0f*Time.deltaTime);
            yield return null;
        }

        player.playerEvents.pummelReleased?.Invoke();
        player.playerEvents.pummelEnds?.Invoke();
    }
    private void MoveToLatchPosition()
    {
        if (!isPummeling || pummelTarget == null) return;

        Vector3 latchPosition = pummelTarget.pummel.GetLatchPosition(latchPositionIndex);
        transform.position = Vector3.Lerp(transform.position, latchPosition, 10.0f * Time.fixedDeltaTime);
    }
    
    public void EjectedByPummelTarget()
    {
        player.playerEvents.pummelEnds?.Invoke();
        player.playerEvents.pummelEjected?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!player.movement.isDashing) return;
        if (!collision.TryGetComponent<Enemy>(out Enemy enemy)) return;
        if (!enemy.isParryStunned || enemy.isBeingPummeled) return;

        player.playerEvents.pummelStarts(enemy);
        enemy.enemyEvents.pummelStarts?.Invoke(player);
    }
}
