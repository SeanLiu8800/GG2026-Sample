using UnityEngine;
using System.Collections.Generic;
public class BulletScript : MonoBehaviour
{
    public Collider2D bulletCollider { get; private set; }
    public SpriteRenderer spriteRenderer { get; private set; }
    [field: Tooltip("Decides what Layer this bullet with interact with")]
    [field: SerializeField] public LayerMask damageLayer { get; private set; }

    [field: Header("Bullet Variables")]
    [field: SerializeField] public BulletStats bulletStats { get; private set; }
    [field: SerializeField, ReadOnly] public GameObject owner { get; private set; }
    [field: SerializeField, ReadOnly] public GameObject target { get; private set; }
    [field: SerializeField, Range(0.0f, 60.0f)] public float moveSpeed { get; private set; } = 5.0f;
    [field: SerializeField, ReadOnly] public Vector3 moveDirection { get; set; } = Vector3.up;
    [SerializeField, ReadOnly] private Vector3 _lookDirection = Vector3.up;
    public Vector3 lookDirection 
    { 
        get { return _lookDirection; }
        private set 
        {
            _lookDirection = value;
            float angle = Mathf.Atan2(_lookDirection.y, _lookDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        } 
    }
    public BulletEvents bulletEvents;
    void Awake()
    {
        if (!TryGetComponent<Collider2D>(out Collider2D _bulletCollider))
        {
            Debug.LogError($"{this.name} DOES NOT have a Collider2D component!");
        }
        if (!TryGetComponent<SpriteRenderer>(out SpriteRenderer _spriteRenderer))
        {
            Debug.LogError($"{this.name} DOES NOT have a SpriteRenderer component!");
        }
        bulletCollider = _bulletCollider;
        spriteRenderer = _spriteRenderer;
    }
    void Start()
    {
        if (!wasInitialized) transform.parent = GameManager.Instance.currRoom.roomBullets.bulletContainer.transform;
        if (owner == null) owner = this.gameObject;
        moveSpeed = bulletStats.initialMoveSpeed;

        damageMultipliers = new List<Bullet_StatMultiplier_Base>();
        parryMultipliers = new List<Bullet_StatMultiplier_Base>();
    }
    
    private bool wasInitialized = false;
    public void Initialize
    (
        GameObject owner,
        GameObject target,
        Vector3 initialMoveDirection = default,
        Vector3 lookDirection = default
    )
    {
        wasInitialized = true;

        this.owner = owner;
        this.target = target;
        this.moveDirection = initialMoveDirection;
        this.lookDirection = lookDirection;
        
        transform.parent = GameManager.Instance.currRoom.roomBullets.bulletContainer.transform;
    }

    public List<Bullet_StatMultiplier_Base> damageMultipliers;
    public float GetDamage()
    {
        float returnDamage = bulletStats.damage;
        foreach (Bullet_StatMultiplier_Base currMultiplier in damageMultipliers) { returnDamage = currMultiplier.Multiply(returnDamage); }
        return returnDamage;
    }

    public List<Bullet_StatMultiplier_Base> parryMultipliers;
    public float GetParry()
    {
        float returnParry = bulletStats.parryValue;
        foreach (Bullet_StatMultiplier_Base currMultiplier in parryMultipliers) returnParry = currMultiplier.Multiply(returnParry);
        return returnParry;
    }
}