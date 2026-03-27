using UnityEngine;

[CreateAssetMenu(fileName = "SoundEffects", menuName = "Scriptable Objects/SoundEffects")]
public class SoundEffects : ScriptableObject
{
    [field: SerializeField] public AudioClip playerDash { get; private set; }
    [field: SerializeField] public AudioClip perfectDash { get; private set; }
    [field: SerializeField] public AudioClip imperfectDash { get; private set; }
    [field: SerializeField] public AudioClip playerDashFails { get; private set; }
    [field: SerializeField] public AudioClip playerAttack { get; private set; }
    [field: SerializeField] public AudioClip playerHurts { get; private set; }
    [field: SerializeField] public AudioClip enemyHurts { get; private set; }
    [field: SerializeField] public AudioClip playerParries { get; private set; }
    [field: SerializeField] public AudioClip enemyParried { get; private set; }
}
