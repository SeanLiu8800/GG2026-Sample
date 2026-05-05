using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSFX", menuName = "Scriptable Objects/PlayerSFX")]
public class PlayerSFX : ScriptableObject
{
    public AudioClip dashStart;
    public AudioClip dashChannel;
    public AudioClip dashEndPerfect;
    public AudioClip dashEndImperfect;

    public AudioClip enhanceAttack;

    public AudioClip takeDamage;
}
