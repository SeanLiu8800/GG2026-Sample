using UnityEngine;

public class Bullet_StatMultiplier_Parry : Bullet_StatMultiplier_Base
{
    public Bullet_StatMultiplier_Parry(int input)
    {
        x = input;
    }
    public override float Multiply(float input)
    {
        Debug.Log("HI FROM PARRY MULTI");
        return input * (x + 1);
    }
}
