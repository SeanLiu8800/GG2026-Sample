using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
public class PlayerAttack : PlayerComponent
{
    private InputAction attackAction;
    [SerializeField] public Collider2D attackArea;
    [SerializeField] private ContactFilter2D attackTargetFilter;

    [field : Header("Attack Variables")]
    [field: SerializeField, ReadOnly] public bool isAttacking { get; private set; } = false;
    [field: SerializeField, ReadOnly] public bool attackIsEnhanced { get; private set; } = false;
    [SerializeField, Range(0, 5)] private int baseDamage = 1;
    [field: SerializeField, Range(0, 5), ReadOnly] public int currDamage { get; private set; } = 1;
    [SerializeField, Range(0.0f, 1.0f)] private float attackDuration = 0.2f;
    private float attackStartTime = 0.0f;
    [field : SerializeField, ReadOnly] public int currAttackID { get; private set; } = 0;
    protected override void Awake()
    {
        base.Awake();

        attackAction = InputSystem.actions.FindAction("Attack");
        attackArea.gameObject.SetActive(false);
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
        if (!player.canAttack || Time.time - attackStartTime < attackDuration + 0.1f) return;

        EnableAttackArea();
        // No target to attack
        if (!CheckAttackArea())
        {
            DisableAttackArea();
            return;
        }

        currAttackID = AttackIDGenerator();
        if (!player.movement.willLunge) player.movement.MultiplyMoveSpeed(0.5f);
        else player.movement.StartAttackLunge();
        AudioManager.Instance.PlaySoundOneShot(AudioManager.Instance.swordSwingSoundEffect);
    }
    /// <summary>
    /// Check to see if there are Colliders within attackArea that follow attackTargetFilter
    /// </summary>
    /// <returns>True if there are applicable Colliders<br/>False if not</returns>
    private bool CheckAttackArea()
    {
        List<Collider2D> currHits = new List<Collider2D>();
        HashSet<Collider2D> totalHits = new HashSet<Collider2D>();
        Vector3 currPos = transform.position;
        Vector3 finalPos = transform.position;
        if (player.movement.willLunge) // Determine finalPos of sliding area checking
        {
            if (player.movement.GetMoveSpeed() > 15.0f) finalPos += (player.movement.lastMovementDirection * 3.0f);
            else finalPos += (player.movement.lastMovementDirection * 2.0f);
        }
        // Angle assumes Collider's Default position, so must calculate it's orientation
        float angleDeg = (attackArea.transform.eulerAngles.z - 360) % 360;
        do
        {
            attackArea.Overlap(currPos, angleDeg, attackTargetFilter, currHits);
            totalHits.UnionWith(currHits);
            currPos = Vector3.MoveTowards(currPos, finalPos, 1.0f);
        }
        while (currPos != finalPos);
        Debug.LogWarning($"{totalHits.Count}, {totalHits.Count >= 1}");
        return totalHits.Count >= 1;
    }
    private void EnableAttackArea()
    {
        isAttacking = true;
        attackArea.gameObject.SetActive(true);
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
        attackArea.gameObject.SetActive(false);
        attackArea.enabled = false;
        attackIsEnhanced = false;
        currDamage = baseDamage;
    }
    
    public void EnhanceAttack()
    {
        attackIsEnhanced = true;
    }
    public bool AttackIsEnhanced() { return attackIsEnhanced; }
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
