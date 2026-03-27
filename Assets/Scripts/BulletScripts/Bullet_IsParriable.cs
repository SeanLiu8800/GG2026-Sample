using UnityEngine;

public class Bullet_IsParriable : Bullet_OnParried_BehaviorBase
{
    protected override void onParriedBehavior()
    {
        if (bullet.owner == null || !bullet.owner.TryGetComponent<Enemy>(out Enemy enemy)) return;

        Debug.Log($"{this.name} IS PARRIED");
        enemy.attack.Parried();
    }
}
