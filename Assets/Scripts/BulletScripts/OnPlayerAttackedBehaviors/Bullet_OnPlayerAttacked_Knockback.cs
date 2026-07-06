using UnityEngine;

public class Bullet_OnPlayerAttacked_Knockback : Bullet_OnPlayerAttacked_BehaviorBase
{
    [Header("Knockback Variables")]
    [SerializeField, Range(0.0f, 10.0f)] private float knockbackDistance = 5.0f;
    private void Start()
    {
        if (knockbackDistance <= 0) Debug.LogWarning($"{knockbackDistance} is Zero! Knockback will not happen!");
    }
    protected override void OnPlayerAttackedBehavior(Player player)
    {
        if (bullet.owner == null) return;
        if (!bullet.owner.TryGetComponent<Enemy>(out Enemy enemy))
        {
            Debug.LogError($"{bullet.owner.name} DOES NOT have an Enemy Component!");
            return;
        }

        enemy.move.Dash(-enemy.toTargetDirection, knockbackDistance);
    }
}
