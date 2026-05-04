using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSFX", menuName = "Scriptable Objects/PlayerSFX")]
public class PlayerSFX : ScriptableObject
{
    public AudioClip dashStart;
    public AudioClip dashChannel;
    public AudioClip dashPerfect;
    public AudioClip dashEnd;

    public AudioClip enhanceAttack;
}
