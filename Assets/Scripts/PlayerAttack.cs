using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
public class PlayerAttack : PlayerComponent
{
    private InputAction attackAction;
    [SerializeField] private Collider2D attackArea;
    [SerializeField] private ContactFilter2D attackTargetFiler;
    protected override void Awake()
    {
        base.Awake();

        attackAction = InputSystem.actions.FindAction("Attack");
    }
    void OnEnable()
    {
        attackAction.canceled += Attack;
    }
    void OnDisable()
    {
        attackAction.canceled -= Attack;
    }
    void FixedUpdate()
    {
        attackArea.transform.rotation = Quaternion.LookRotation(Vector3.forward, player.movement.lastMovementDirection);
    }

    private void Attack(InputAction.CallbackContext context)
    {
        Debug.Log("Player attempts attack");
        List<Collider2D> hits = new List<Collider2D>();
        if (attackArea.Overlap(attackTargetFiler, hits) > 0)
        {
            Debug.Log("Player Attacks!");
            foreach (Collider2D currCollider in hits)
            {
                Debug.Log($"Hit {currCollider.name}");
            }
        }
    }
}
