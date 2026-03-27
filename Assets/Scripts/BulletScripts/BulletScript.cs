using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField, ReadOnly] private Collider2D bulletCollider;
    [SerializeField, ReadOnly] private Rigidbody2D bulletRigidbody;
    [SerializeField] private LayerMask layerMask;

    [field : Header("Bullet Variables")]
    [field : SerializeField, ReadOnly] public GameObject owner { get; private set; }
    [field : SerializeField, ReadOnly] public GameObject target { get; private set; }
    [SerializeField, Range(0, 5)] private int damage = 1;
    [SerializeField, Range(0, 5)] private int empowerRate = 1;

    public BulletEvents bulletEvents;
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

    public void Initialize(GameObject owner, GameObject target, Vector3 initialLinearVelocity = default)
    {
        this.owner = owner;
        this.target = target;
        SetLinearVelocity(initialLinearVelocity);
    }
    public void SetLinearVelocity(Vector3 input)
    {
        bulletRigidbody.linearVelocity = input;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & layerMask) == 0) return;

        // If Bullet hits Player or Player's Attack area
        if (TryCheckIfPlayerCollider(collision, out Player player))
        {
            if (PlayerCollision(player)) Destroy(this.gameObject);
        }
        else if (collision.TryGetComponent(out IDamageable iDamageable))
        {
            iDamageable.Damage(damage);
            Destroy(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }        
    }
    /// <summary>
    /// Checks whether input Collider2D is from the Player
    /// </summary>
    /// <param name="collision">The Collider2D to check</param>
    /// <param name="player">The corresponding Player script from the Collider</param>
    /// <returns>
    /// Returns True and the corresponding Player script if the Collider2D is from the Player<br/>
    /// Returns False otherwise
    /// </returns>
    private bool TryCheckIfPlayerCollider(Collider2D collision, out Player player)
    {
        // If collider is directly from Player GameObject
        if (collision.TryGetComponent<Player>(out player)) return true;
        // If collider is from Player's Attack Area
        else if (collision.transform.parent != null && collision.transform.parent.TryGetComponent<Player>(out player)) return true;
        else return false;
    }
    /// <summary>
    /// Performs logic when Player collides with this bullet
    /// </summary>
    /// <param name="player">Player to attack</param>
    /// <returns>Returns True if the bullet interacts with Player, False otherwise (like if Player is Invincible)</returns>
    private bool PlayerCollision(Player player)
    {
        Debug.Log("Player Collision!");
        if (player.movement.isDashing)
        {
            if (bulletRigidbody.linearVelocity == Vector2.zero ||
            Vector3.Dot(bulletRigidbody.linearVelocity.normalized, player.movement.dashDirection) < -0.7f)
                player.attack.Empower(empowerRate);
        }
        else if (player.attack.isAttacking)
        {
            if (player.attack.AttackIsEnhanced()) bulletEvents.onEnhancedAttacked?.Invoke();
            else return false;
        }
        else
        {
            if (player.health.isInvincible) return false;
            else player.health.Damage(damage);
        }
        return true;
    }
}
