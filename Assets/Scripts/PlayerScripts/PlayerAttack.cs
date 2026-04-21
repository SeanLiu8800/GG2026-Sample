using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
public class PlayerAttack : PlayerComponent
{
    private InputAction attackAction;
    [SerializeField] private GameObject playerAttack;
    [SerializeField] private GameObject playerEnhancedAttack;
    [SerializeField] private ContactFilter2D attackTargetFilter;

    [field: Header("Attack Variables")]
    [field: SerializeField, ReadOnly] public bool isAttacking { get; private set; } = false;
    [field: SerializeField, ReadOnly] public bool attackIsEnhanced { get; private set; } = false;
    [SerializeField, ReadOnly] private bool attackParries = false;
    [SerializeField, Range(0, 5)] private int baseDamage = 1;
    [field: SerializeField, Range(0, 5), ReadOnly] public int currDamage { get; private set; } = 1;
    [SerializeField, Range(0.0f, 1.0f)] private float attackDuration = 0.2f;
    private float attackStartTime = 0.0f;
    
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
    
    #region ----- Event Functions -----
    void EnhanceAttack()
    {
        attackIsEnhanced = true;
        AudioManager.Instance.PlaySoundOneShot(AudioManager.Instance.soundEffects.playerEnhancesAttack);
    }
    void AttackStarts()
    {
        isAttacking = true;
        attackStartTime = Time.time;
        attackParries = false;
    }
    void OnParry()
    {
        attackParries = true;
    }
    void AttackEnds()
    {
        isAttacking = false;
        currDamage = baseDamage;
        if (!attackParries) attackIsEnhanced = false; // Unenhance attack if player DOES NOT parry
    }
    #endregion
    void FixedUpdate()
    {
        UpdateAttackArea();
    }
    
    private void Attack(InputAction.CallbackContext context)
    {
        if (!player.allowAttack || (isAttacking && !attackParries)) return;
        if (player.pummel.isPummeling || player.move.isKnockbacked) return;
        if (player.autoEnhance) player.playerEvents.enhanceAttack?.Invoke();
        if (attackParries) currDamage = baseDamage; // Reset Damage if attack starts before it's ended due to parry

        BulletScript bullet = Instantiate(attackIsEnhanced ? playerEnhancedAttack : playerAttack).GetComponent<BulletScript>();
        bullet.Initialize(this.gameObject, null, player.move.lastMovementDirection, player.move.lastMovementDirection);
        bullet.damage = currDamage;
        // No target to attack
        if (!CheckAttackArea(bullet.bulletCollider))
        {
            Destroy(bullet.gameObject);
            return;
        }

        player.playerEvents.attackStarts?.Invoke();
    }
    /// <summary>
    /// Check to see if there are Colliders within attackArea's Collider2D that follow attackTargetFilter
    /// </summary>
    /// <returns>True if there are applicable Colliders<br/>False if not</returns>
    private bool CheckAttackArea(Collider2D collider)
    {
        List<Collider2D> currHits = new List<Collider2D>();
        Vector3 currPos = transform.position;
        Vector3 finalPos = transform.position;
        if (player.allowLunge && (player.move.willLunge || player.autoLunge)) 
            finalPos = currPos + player.move.lastMovementDirection * player.move.lungeDistance;

        // Angle assumes Collider's Default position, so must calculate it's orientation
        float angleDeg = (collider.transform.eulerAngles.z - 360) % 360;
        // Slide Player Attack Area towards finalPos, with increments of 0.5f units
        // It should be using a While loop, but I used a for loop to limit how many times it runs
        for (int i = 0; i < 10; i ++)
        {
            collider.Overlap(currPos, angleDeg, attackTargetFilter, currHits);
            if (CountValidColliders(currHits) >= 1) return true;
            if (currPos == finalPos) break;
            currPos = Vector3.MoveTowards(currPos, finalPos, 0.5f);
        }
        return false;
    }
    /// <summary>
    /// Returns the number of colliders in the List input that should be attacked by the player
    /// </summary>
    /// <param name="input">Input List of Colliders</param>
    /// <returns>The number of Colliders the player should attack</returns>
    private int CountValidColliders(List<Collider2D> input)
    {
        int count = 0;
        for (int i = 0; i < input.Count; i ++)
        {
            // Probably an authorized attackArea
            if (!input[i].TryGetComponent<Enemy>(out Enemy currEnemy)) count++;
            // Normal Enemy
            else if (currEnemy.health.currHealth > 0) count++;
        }
        return count;
    }
    private void UpdateAttack()
    {
        if (!isAttacking || Time.time - attackStartTime < attackDuration) return;
        player.playerEvents.attackEnds?.Invoke();
    }
    public void Empower(int input = 1)
    {
        //Debug.Log("Empowering Player's Attack!");
        currDamage += input;
    }
}
