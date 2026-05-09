using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class PlayerPummel : PlayerComponent
{
    private InputAction moveAction;
    private InputAction pummelAction;
    private LayerMask enemyLayer;
    [SerializeField, Range(0.0f, 1.5f)] private float checkRadius = 0.75f;

    [field: Header("Pummel Variables")]
    [SerializeField, ReadOnly] private Enemy pummelTarget;
    [SerializeField, ReadOnly] private Vector3 pummelDismountLocation = Vector3.zero;
    private bool ignoreFirstPummel = true;
    protected override void Awake()
    {
        base.Awake();

        moveAction = InputSystem.actions.FindAction("Move");
        pummelAction = InputSystem.actions.FindAction("PlayerPummelControls/Pummel");

        if ((enemyLayer = LayerMask.GetMask("Enemy")) == 0) Debug.LogError("Enemy Layer NOT FOUND!");
    }
    private void OnEnable()
    {
        pummelAction.started += DecideAction;
        pummelAction.canceled += DecideAction;

        player.playerEvents.dashStarts += DashStarts;

        player.playerEvents.pummelStarts += PummelStarts;
        player.playerEvents.pummelEnds += PummelEnds;
    }
    private void OnDisable()
    {
        pummelAction.started -= DecideAction;
        pummelAction.canceled -= DecideAction;

        player.playerEvents.dashStarts -= DashStarts;

        player.playerEvents.pummelStarts -= PummelStarts;
        player.playerEvents.pummelEnds -= PummelEnds;
    }
    
    #region ----- Event Functions -----
    void DashStarts()
    {
        pummelDismountLocation = transform.position;
        StartCoroutine(FindPummelTarget());
    }
    void PummelStarts(Enemy enemy)
    {
        player.AddState(PlayerState.Pummeling);
        pummelTarget = enemy;

        player.playerCollider.enabled = false;
        ignoreFirstPummel = true;
    }
    void PummelEnds()
    {
        player.RemoveState(PlayerState.Pummeling);
        this.pummelTarget = null;
        player.playerCollider.enabled = true;
    }
    #endregion
    
    void FixedUpdate()
    {
        MoveToLatchPosition();
    }
    private IEnumerator FindPummelTarget()
    {
        while (player.isDashing)
        {
            Collider2D hit = Physics2D.OverlapCircle(transform.position, checkRadius, enemyLayer);
            if (hit != null && hit.TryGetComponent<Enemy>(out Enemy enemy))
            {
                if (enemy.IsPummelable) 
                {
                    yield return null;
                    player.playerEvents.pummelStarts?.Invoke(enemy);
                    enemy.enemyEvents.pummelStarts?.Invoke(player);
                } 
            }
            yield return null;
        }
    }
    private void DecideAction(InputAction.CallbackContext context)
    {
        if (!player.isPummeling || pummelTarget == null) return;
        if (context.started)
        {
            ignoreFirstPummel = false;
            Vector2 toTarget = pummelTarget.transform.position - transform.position;
            Vector3 directionInput = moveAction.ReadValue<Vector2>();
            if (Vector2.Dot(toTarget.normalized, directionInput.normalized) < -0.6) ReleaseTarget();
            else Pummel();
        }
        else Pummel();
    }    
    private void Pummel()
    {
        if (ignoreFirstPummel) return;

        pummelTarget.health.Damage(1.0f, DamageElement.None, 0.0f, this.gameObject);
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
        player.playerEvents.pummelDismount?.Invoke();
        player.playerEvents.pummelEnds?.Invoke();
    }
    private void MoveToLatchPosition()
    {
        if (!player.isPummeling || pummelTarget == null) return;

        Vector3 latchPosition = pummelTarget.pummel.GetClosestLatchPoint(transform.position);
        transform.position = Vector3.Lerp(transform.position, latchPosition, 10.0f * Time.fixedDeltaTime);
    }
    public void EjectedByPummelTarget()
    {
        player.playerEvents.pummelEnds?.Invoke();
        player.playerEvents.pummelEjected?.Invoke();
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.DrawSphere(pummelDismountLocation, 0.2f);
        if (player.isDashing) Gizmos.DrawSphere(transform.position, checkRadius);
    }
}
