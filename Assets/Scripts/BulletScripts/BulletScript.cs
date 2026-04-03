using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [field: SerializeField, ReadOnly] public Collider2D bulletCollider { get; private set; }
    [field: SerializeField, ReadOnly] public Rigidbody2D bulletRigidbody { get; private set; }
    [field: Tooltip("Decides what Layer this bullet with interact with")]
    [field: SerializeField] public LayerMask layerMask { get; private set; }

    [field: Header("Bullet Variables")]
    [field: SerializeField, ReadOnly] public GameObject owner { get; private set; }
    [field: SerializeField, ReadOnly] public GameObject target { get; private set; }
    [field: SerializeField, ReadOnly] public Vector3 initialLinearVelocity { get; private set; }
    [field: SerializeField, ReadOnly] public Vector3 lookDirection { get; private set; }
    [SerializeField, Range(0, 5)] public int damage = 1;
    [SerializeField, Range(0, 5)] public int empowerRate = 1;

    public BulletEvents bulletEvents;
    void Awake()
    {
        if (!TryGetComponent<Collider2D>(out Collider2D _bulletCollider))
        {
            Debug.LogError($"{this.name} DOES NOT have a Collider2D component!");
        }
        bulletCollider = _bulletCollider;
        if (!TryGetComponent<Rigidbody2D>(out Rigidbody2D _bulletRigidbody))
        {
            Debug.LogError($"{this.name} DOES NOT have a Rigidbody2D component!");
        }
        bulletRigidbody = _bulletRigidbody;
    }

    public void Initialize
    (
        GameObject owner, 
        GameObject target, 
        Vector3 initialLinearVelocity = default, 
        Vector3 lookDirection = default
    )
    {
        this.owner = owner;
        this.target = target;
        this.initialLinearVelocity = initialLinearVelocity;
        this.lookDirection = lookDirection;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
}
