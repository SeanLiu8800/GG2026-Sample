using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField, ReadOnly] private Collider2D bulletCollider;
    [SerializeField, ReadOnly] private Rigidbody2D bulletRigidbody;
    [SerializeField] private LayerMask layerMask;

    [SerializeField, Range(0, 5)] private int damage = 1;
    [SerializeField, Range(0, 5)] private int empowerRate = 1;
    void Awake()
    {
        if (!TryGetComponent<Collider2D>(out bulletCollider))
        {
            Debug.LogError($"{this.name} DOES NOT have a Collider2D component!");
        }
        if (!TryGetComponent<Rigidbody2D>(out bulletRigidbody))
        {
            Debug.LogError($"{this.name} DOES NOT have a Rigidbody2D component!");
        }
    }

    public void SetLinearVelocity(Vector3 input)
    {
        bulletRigidbody.linearVelocity = input;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & layerMask) == 0) return;

        if (collision.TryGetComponent<Player>(out Player player)) PlayerCollision(player);
        else if (collision.TryGetComponent(out IDamageable iDamageable)) iDamageable.Damage(damage);

        Destroy(this.gameObject);
    }
    private void PlayerCollision(Player player)
    {
        if (player.movement.isDashing)
        {
            if (bulletRigidbody.linearVelocity == Vector2.zero ||
            Vector3.Dot(bulletRigidbody.linearVelocity.normalized, player.movement.dashDirection) < -0.7f)
                player.attack.Empower(empowerRate);
        }
        else player.health.Damage(damage);
    }
}
