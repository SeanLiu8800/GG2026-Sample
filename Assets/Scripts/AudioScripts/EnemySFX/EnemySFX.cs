using UnityEngine;

[CreateAssetMenu(fileName = "EnemySFX", menuName = "Scriptable Objects/EnemySFX")]
public class EnemySFX : ScriptableObject
{
    public AudioClip takeDamage;
    [Range(0.0f, 1.0f)] public float takeDamageVolume = 1.0f;
    public AudioClip parryStunned;
    [Range(0.0f, 1.0f)] public float parryStunnedVolume = 1.0f;
}
