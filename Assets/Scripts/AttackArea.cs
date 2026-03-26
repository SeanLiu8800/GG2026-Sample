using UnityEngine;

public class AttackArea : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D attackCollider;

    [SerializeField] private bool spriteAlwaysOn = false;
    private void Awake()
    {
        if (!TryGetComponent<SpriteRenderer>(out spriteRenderer))
            Debug.LogError($"{this.name} DOES NOT have a SpriteRenderer Component!");
        if (!TryGetComponent<Collider2D>(out attackCollider))
            Debug.LogError($"{this.name} DOES NOT have a Collider2D Component!");

        DisableAttack();
    }

    public SpriteRenderer GetSpriteRenderer() { return spriteRenderer; }
    public Collider2D GetCollider2D() { return attackCollider; }
    public void EnableAttack()
    {
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (attackCollider != null) attackCollider.enabled = true;
    }
    public void DisableAttack()
    {
        if (spriteRenderer != null && !spriteAlwaysOn) spriteRenderer.enabled = false;
        if (attackCollider != null) attackCollider.enabled = false;
    }
}
