using UnityEngine;

public class Bullet_StatMultiplier_Damage : Bullet_StatMultiplier_Base
{
    public Bullet_StatMultiplier_Damage(int input)
    {
        x = input;
    }
    public override float Multiply(float input)
    {
        return input * (x + 1);
    }
}
