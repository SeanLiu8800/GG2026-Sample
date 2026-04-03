using UnityEngine;

public class Bullet_OnEnhancedAttacked_Knockback : Bullet_OnEnhancedAttacked_BehaviorBase
{
    [Header("Knockback Variables")]
    [SerializeField, Range(0.0f, 10.0f)] private float knockbackStrength = 5.0f;
    private void Start()
    {
        if (knockbackStrength <= 0) Debug.LogWarning($"{knockbackStrength} is Zero or Negative! Knockback will not happen!");
    }
    protected override void OnEnhancedAttackedBehavior(Player player)
    {
        if (!bullet.owner.TryGetComponent<Enemy>(out Enemy enemy))
        {
            Debug.LogError($"{bullet.owner.name} DOES NOT have an Enemy Component!");
            return;
        }

        Vector3 direction = (enemy.transform.position - player.transform.position).normalized;
        enemy.enemyRigidbody.AddForce(direction * knockbackStrength, ForceMode2D.Impulse);
    }
}
