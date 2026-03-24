using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
public class PlayerAttack : PlayerComponent
{
    private InputAction attackAction;
    [SerializeField] private Collider2D attackArea;
    [SerializeField] private ContactFilter2D attackTargetFiler;

    [SerializeField] private bool attackIsActive = false;
    private float attackStartTime = 0.0f;
    [SerializeField, Range(0.0f, 1.0f)] private float attackDuration = 0.2f;
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
        UpdateAttackArea();
    }

    private void Attack(InputAction.CallbackContext context)
    {
        EnableAttackArea();
        List<Collider2D> hits = new List<Collider2D>();
        // No target to attack
        if (attackArea.Overlap(attackTargetFiler, hits) <= 0)
        {
            DisableAttackArea();
            return;
        }

        AudioManager.Instance.PlaySoundOneShot(AudioManager.Instance.swordSwingSoundEffect);
        foreach (Collider2D currCollider in hits)
        {
            Debug.Log($"Hits {currCollider.name}");
        }
    }
    private void EnableAttackArea()
    {
        Debug.Log("Enable Attack");
        attackIsActive = true;
        attackArea.enabled = true;
        attackStartTime = Time.time;

        attackArea.transform.rotation = Quaternion.LookRotation(Vector3.forward, player.movement.lastMovementDirection);
    }
    private void UpdateAttackArea()
    {
        if (!attackIsActive || Time.time - attackStartTime < attackDuration) return;
        DisableAttackArea();
    }
    private void DisableAttackArea()
    {
        Debug.Log("Disable Attack");
        attackIsActive = false;
        attackArea.enabled = false;
    }
}
