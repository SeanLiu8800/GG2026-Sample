using UnityEngine;

public class BulletScript : MonoBehaviour
{
    [SerializeField, ReadOnly] private Collider2D bulletCollider;
    [SerializeField, ReadOnly] private Rigidbody2D bulletRigidbody;

    [field: Header("Bullet Variables")]
    [field: SerializeField, ReadOnly] public GameObject owner { get; private set; }
    [field: SerializeField, ReadOnly] public GameObject target { get; private set; }
    [SerializeField, Range(0, 5)] public int damage = 1;
    [SerializeField, Range(0, 5)] public int empowerRate = 1;

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
        SetLinearVelocity(initialLinearVelocity);
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }
    public void SetLinearVelocity(Vector3 input)
    {
        bulletRigidbody.linearVelocity = input;
    }
}
