using UnityEngine;

public class Bullet_Multiplier_DashDestroy : BulletComponent
{
    [SerializeField, Range(0, 10)] private int dashDestroyCount = 0;

    public void Initialize(int dashDestroyCount = 0)
    {
        this.dashDestroyCount = dashDestroyCount;
    }
}
