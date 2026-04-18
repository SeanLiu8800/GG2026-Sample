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
    [SerializeField, ReadOnly] private Vector3 pummelDismountLocation = Vector3.zero;
    
    protected override void Awake()
    {
        base.Awake();

        moveAction = InputSystem.actions.FindAction("Move");
        pummelAction = InputSystem.actions.FindAction("PlayerPummelControls/Pummel");
    }
    private void OnEnable()
    {
        pummelAction.started += DecideAction;

        player.playerEvents.dashStarts += DashStarts;

        player.playerEvents.pummelStarts += PummelStarts;
        player.playerEvents.pummelEnds += PummelEnds;
        player.playerEvents.pummelReleased += PummelReleased;
        player.playerEvents.pummelEjected += PummelEjected;
    }
    private void OnDisable()
    {
        pummelAction.started -= DecideAction;

        player.playerEvents.dashStarts -= DashStarts;

        player.playerEvents.pummelStarts -= PummelStarts;
        player.playerEvents.pummelEnds -= PummelEnds;
        player.playerEvents.pummelReleased -= PummelReleased;
        player.playerEvents.pummelEjected -= PummelEjected;
    }
    
    #region ----- Event Functions -----
    void DashStarts()
    {
        pummelDismountLocation = transform.position;
    }
    void PummelStarts(Enemy enemy)
    {
        isPummeling = true;
        pummelTarget = enemy;

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
            //Debug.LogWarning($"Releasing {pummelTarget.name}");
            ReleaseTarget();
        }
        else
        {
            //Debug.LogWarning($"Pummeling {pummelTarget.name}");
            Pummel();
        }
    }    
    private void Pummel()
    {
        pummelTarget.health.Damage(1, this.gameObject);
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
        Vector3 dismountEndPosition = pummelDismountLocation;
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

        Vector3 latchPosition = pummelTarget.pummel.GetClosestLatchPoint(transform.position);
        transform.position = Vector3.Lerp(transform.position, latchPosition, 10.0f * Time.fixedDeltaTime);
    }
    
    public void EjectedByPummelTarget()
    {
        player.playerEvents.pummelEnds?.Invoke();
        player.playerEvents.pummelEjected?.Invoke();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!player.move.isDashing) return;
        if (!collision.TryGetComponent<Enemy>(out Enemy enemy)) return;
        if (!enemy.allowInstantPummel && (!enemy.isParryStunned || enemy.isBeingPummeled)) return;

        player.playerEvents.pummelStarts(enemy);
        enemy.enemyEvents.pummelStarts?.Invoke(player);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.DrawSphere(pummelDismountLocation, 0.2f);
    }
}
