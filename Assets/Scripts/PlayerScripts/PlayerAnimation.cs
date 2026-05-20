using UnityEngine;

public class PlayerAnimation : PlayerComponent
{
    [SerializeField] private Animator animator;
    void Update()
    {
        Animate();
    }

    private void Animate()
    {
        if (player.playerRigidbody.linearVelocity == Vector2.zero) return;
        animator.SetFloat("LastMoveX", player.playerRigidbody.linearVelocityX);
        animator.SetFloat("LastMoveY", player.playerRigidbody.linearVelocityY);
    }
}
