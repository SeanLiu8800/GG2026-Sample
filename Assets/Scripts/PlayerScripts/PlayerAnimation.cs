using UnityEngine;

public class PlayerAnimation : PlayerComponent
{
    [SerializeField] private Animator animator;
    private Enemy pummelTarget;
    private void OnEnable()
    {
        player.playerEvents.dashStarts += DashStarts;
        player.playerEvents.dashEnds += DashEnds;

        player.playerEvents.attackStarts += AttackStarts;
        player.playerEvents.attackEnds += AttackEnds;

        player.playerEvents.pummelStarts += PummelStarts;
        player.playerEvents.pummelEnds += PummelEnds;
    }
    private void OnDisable()
    {
        player.playerEvents.dashStarts -= DashStarts;
        player.playerEvents.dashEnds -= DashEnds;

        player.playerEvents.attackStarts -= AttackStarts;
        player.playerEvents.attackEnds -= AttackEnds;

        player.playerEvents.pummelStarts -= PummelStarts;
        player.playerEvents.pummelEnds -= PummelEnds;
    }
    #region ----- Event Functions -----
    void DashStarts()
    {
        animator.SetBool("IsDashing", true);
    }
    void DashEnds()
    {
        animator.SetBool("IsDashing", false);
    }
    void AttackStarts()
    {
        animator.SetBool("IsAttacking", true);
        animator.SetFloat("AttackDirectionX", player.move.lastMovementDirection.x);
        animator.SetFloat("AttackDirectionY", player.move.lastMovementDirection.y);
    }
    void AttackEnds()
    {
        animator.SetBool("IsAttacking", false);
    }
    void PummelStarts(Enemy pummelTarget)
    {
        this.pummelTarget = pummelTarget;
        animator.SetBool("IsPummeling", true);
    }
    void PummelEnds()
    {
        this.pummelTarget = null;
        animator.SetBool("IsPummeling", false);
    }
    
    #endregion
    void Update()
    {
        Animate();
    }

    private void Animate()
    {
        if (player.playerRigidbody.linearVelocity != Vector2.zero)
        {
            animator.SetFloat("LastMoveX", player.playerRigidbody.linearVelocityX);
            animator.SetFloat("LastMoveY", player.playerRigidbody.linearVelocityY);
        }

        if (pummelTarget != null) animator.SetFloat("PummelDirection", pummelTarget.transform.position.x - transform.position.x);
    }
}
