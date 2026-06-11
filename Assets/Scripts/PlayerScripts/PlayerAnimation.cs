using UnityEngine;

public class PlayerAnimation : PlayerComponent
{
    [SerializeField] private Animator animator;
    private Enemy pummelTarget;
    private void OnEnable()
    {
        player.playerEvents.pummelStarts += PummelStarts;
        player.playerEvents.pummelEnds += PummelEnds;
    }
    private void OnDisable()
    {
        player.playerEvents.pummelStarts -= PummelStarts;
        player.playerEvents.pummelEnds -= PummelEnds;
    }
    #region ----- Event Functions -----
    void PummelStarts(Enemy pummelTarget)
    {
        this.pummelTarget = pummelTarget;
        animator.SetTrigger("IsPummeling");
    }
    void PummelEnds()
    {
        this.pummelTarget = null;
        animator.ResetTrigger("IsPummeling");
        animator.SetTrigger("IsPummeling");
    }
    #endregion
    void Update()
    {
        Animate();
    }

    private void Animate()
    {
        animator.SetFloat("LastMoveX", player.playerRigidbody.linearVelocityX);
        animator.SetFloat("LastMoveY", player.playerRigidbody.linearVelocityY);

        if (pummelTarget == null) return;
        animator.SetFloat("PummelDirection", pummelTarget.transform.position.x - transform.position.x);
    }
}
