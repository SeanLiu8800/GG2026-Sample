using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
public class PlayerAttack : PlayerComponent
{
    private InputAction attackAction;
    [SerializeField] private Collider2D attackArea;
    [SerializeField] private ContactFilter2D attackTargetFiler;

    [field : Header("Attack Variables")]
    [field : SerializeField, ReadOnly] public bool attackIsActive { get; private set; } = false;
    [SerializeField, Range(0.0f, 1.0f)] private float attackDuration = 0.2f;
    private float attackStartTime = 0.0f;
    protected override void Awake()
    {
        base.Awake();

        attackAction = InputSystem.actions.FindAction("Attack");
        attackArea.enabled = false;
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

        player.movement.StartAttackLunge();
        AudioManager.Instance.PlaySoundOneShot(AudioManager.Instance.swordSwingSoundEffect);
        foreach (Collider2D currCollider in hits)
        {
            Debug.Log($"Hits {currCollider.name}");
        }
    }
    private void EnableAttackArea()
    {
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
        attackIsActive = false;
        attackArea.enabled = false;
    }
}
