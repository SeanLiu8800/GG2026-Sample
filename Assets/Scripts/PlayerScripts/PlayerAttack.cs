using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
public class PlayerAttack : PlayerComponent
{
    private InputAction attackAction;
    [SerializeField] private AttackArea attackArea;
    [SerializeField] private ContactFilter2D attackTargetFilter;

    [field : Header("Attack Variables")]
    [field: SerializeField, ReadOnly] public bool isAttacking { get; private set; } = false;
    [field: SerializeField, ReadOnly] public bool attackIsEnhanced { get; private set; } = false;
    [field: SerializeField, ReadOnly] private bool attackParries = false;
    [SerializeField, Range(0, 5)] private int baseDamage = 1;
    [field: SerializeField, Range(0, 5), ReadOnly] public int currDamage { get; private set; } = 1;
    [SerializeField, Range(0.0f, 1.0f)] private float attackDuration = 0.2f;
    private float attackStartTime = 0.0f;
    [field : SerializeField, ReadOnly] public int currAttackID { get; private set; } = 0;
    protected override void Awake()
    {
        base.Awake();

        attackAction = InputSystem.actions.FindAction("Attack");
    }
    void OnEnable()
    {
        attackAction.canceled += Attack;

        player.playerEvents.enhanceAttack += EnhanceAttack;

        player.playerEvents.attackStarts += AttackStarts;
        player.playerEvents.onParry += OnParry;
        player.playerEvents.attackEnds += AttackEnds;
    }
    void OnDisable()
    {
        attackAction.canceled -= Attack;

        player.playerEvents.enhanceAttack -= EnhanceAttack;

        player.playerEvents.attackStarts -= AttackStarts;
        player.playerEvents.onParry -= OnParry;
        player.playerEvents.attackEnds -= AttackEnds;
    }
    
    #region Event Functions
    void EnhanceAttack()
    {
        attackIsEnhanced = true;
        attackArea.GetSpriteRenderer().SetColor(Color.cyan.r, Color.cyan.g, Color.cyan.b, -1.0f);
        AudioManager.Instance.PlaySoundOneShot(AudioManager.Instance.soundEffects.playerEnhancesAttack);
    }
    void AttackStarts()
    {
        isAttacking = true;
        attackStartTime = Time.time;
        currAttackID = AttackIDGenerator();
        attackParries = false;
        if (attackIsEnhanced) AudioManager.Instance.PlaySoundOneShot(AudioManager.Instance.soundEffects.playerAttackEnhanced);
        else AudioManager.Instance.PlaySoundOneShot(AudioManager.Instance.soundEffects.playerAttack);
    }
    void OnParry()
    {
        attackParries = true;
    }
    void AttackEnds()
    {
        isAttacking = false;
        currDamage = baseDamage;
        attackArea.DisableAttack();
        if (!attackParries) // Unenhance attack if player DOES NOT parry
        {
            attackParries = false;
            attackIsEnhanced = false;
            attackArea.GetSpriteRenderer().SetColor(Color.white.r, Color.white.g, Color.white.b, -1.0f);
        }
    }
    #endregion

    void FixedUpdate()
    {
        UpdateAttackArea();
    }
    
    private void Attack(InputAction.CallbackContext context)
    {
        if (!player.canAttack || Time.time - attackStartTime < attackDuration + 0.1f) return;

        attackArea.EnableAttack();
        attackArea.transform.rotation = Quaternion.LookRotation(Vector3.forward, player.movement.lastMovementDirection);
        // No target to attack
        if (!CheckAttackArea())
        {
            attackArea.DisableAttack();
            return;
        }

        player.playerEvents.attackStarts?.Invoke();
    }
    /// <summary>
    /// Check to see if there are Colliders within attackArea's Collider2D that follow attackTargetFilter
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
            attackArea.GetCollider2D().Overlap(currPos, angleDeg, attackTargetFilter, currHits);
            totalHits.UnionWith(currHits);
            currPos = Vector3.MoveTowards(currPos, finalPos, 1.0f);
        }
        while (currPos != finalPos);
        //Debug.LogWarning($"{totalHits.Count}, {totalHits.Count >= 1}");
        return totalHits.Count >= 1;
    }
    private void UpdateAttackArea()
    {
        if (!isAttacking || Time.time - attackStartTime < attackDuration) return;
        player.playerEvents.attackEnds?.Invoke();
        
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
