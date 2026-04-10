using UnityEngine;

public class Bullet_SpawnAttackArea : BulletComponent
{
    [Header("Attack Area Variables")]
    [SerializeField, Range(0.0f, 5.0f)] private float length = 2.0f;
    [SerializeField, Range(0.0f, 20.0f)] private float height = 2.0f;
    [SerializeField, Range(0.0f, 5.0f)] private float duration = 2.0f;
    void Start()
    {
        AttackZoneManager.Instance.SetSquareAttackZone(
            transform.position + bullet.moveDirection * (height * 0.5f),
            bullet.moveDirection,
            length,
            height,
            duration
        );
    }
}
