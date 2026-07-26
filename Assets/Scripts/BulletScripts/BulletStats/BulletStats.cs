using UnityEngine;

[CreateAssetMenu(fileName = "BulletStats", menuName = "Scriptable Objects/BulletStats")]
public class BulletStats : ScriptableObject
{
    [Range(0.0f, 60.0f)] public float initialMoveSpeed = 5.0f;

    public float damage = 1.0f;
    public DamageElement element = DamageElement.None;
    [Range(0.0f, 5.0f)] public float elementBuildup = 1.0f;
    [Range(-1.0f, 3.0f)] public float invincibilityTime = -1.0f;

    [Range(0.0f, 5.0f)] public float parryValue = 1.0f;

    [Range(0.0f, 10.0f)] public float engineDrainAmount = 0.0f;
}
