using UnityEngine;

[CreateAssetMenu(fileName = "EnemySFX", menuName = "Scriptable Objects/EnemySFX")]
public class EnemySFX : ScriptableObject
{
    public AudioClip takeDamage;

    public AudioClip parryStunned;
}
