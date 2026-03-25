using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
public class PlayerAttack : PlayerComponent
{
    private InputAction attackAction;
    [SerializeField] public Collider2D attackArea;
    [SerializeField] private ContactFilter2D attackTargetFiler;

    [field : Header("Attack Variables")]
    [field: SerializeField, ReadOnly] public bool isAttacking { get; private set; } = false;
    [SerializeField, Range(0, 5)] private int baseDamage = 1;
    [field: SerializeField, Range(0, 5), ReadOnly] public int currDamage { get; private set; } = 1;
    [SerializeField, Range(0.0f, 1.0f)] private float attackDuration = 0.2f;
    private float attackStartTime = 0.0f;
    [field : SerializeField, ReadOnly] public int currAttackID { get; private set; } = 0;
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
        if (Time.time - attackStartTime < attackDuration+0.1f) return;

        EnableAttackArea();
        List<Collider2D> hits = new List<Collider2D>();
        // No target to attack
        if (attackArea.Overlap(attackTargetFiler, hits) <= 0)
        {
            DisableAttackArea();
            return;
        }

        currAttackID = AttackIDGenerator();
        player.movement.StartAttackLunge();
        AudioManager.Instance.PlaySoundOneShot(AudioManager.Instance.swordSwingSoundEffect);
    }
    private void EnableAttackArea()
    {
        isAttacking = true;
        attackArea.enabled = true;
        attackStartTime = Time.time;

        attackArea.transform.rotation = Quaternion.LookRotation(Vector3.forward, player.movement.lastMovementDirection);
    }
    private void UpdateAttackArea()
    {
        if (!isAttacking || Time.time - attackStartTime < attackDuration) return;
        DisableAttackArea();
    }
    private void DisableAttackArea()
    {
        isAttacking = false;
        attackArea.enabled = false;
    }
    public void ResetDamage()
    {
        currDamage = baseDamage;
    }
    public void Empower(int input = 1)
    {
        Debug.Log("Empowering Player's Attack!");
        currDamage += input;
    }
    private int AttackIDGenerator()
    {
        return (currAttackID = (currAttackID + 1) % 256);
    }
}
