using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSFX", menuName = "Scriptable Objects/PlayerSFX")]
public class PlayerSFX : ScriptableObject
{
    public AudioClip dashStart;
    [Range(0.0f, 1.0f)] public float dashStartVolume = 1.0f; 
    public AudioClip dashChannel;
    [Range(0.0f, 1.0f)] public float dashChannelVolume = 1.0f;
    public AudioClip dashEndPerfect;
    [Range(0.0f, 1.0f)] public float dashEndPerfectVolume = 1.0f;
    public AudioClip dashEndImperfect;
    [Range(0.0f, 1.0f)] public float dashEndImperfectVolume = 1.0f;

    public AudioClip enhanceAttack;
    [Range(0.0f, 1.0f)] public float enhanceAttackVolume = 1.0f;
    public AudioClip takeDamage;
    [Range(0.0f, 1.0f)] public float takeDamageVolume = 1.0f;
}
