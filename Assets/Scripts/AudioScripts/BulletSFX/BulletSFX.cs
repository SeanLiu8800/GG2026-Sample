using UnityEngine;

[CreateAssetMenu(fileName = "BulletSFX", menuName = "Scriptable Objects/BulletSFX")]
public class BulletSFX : ScriptableObject
{
    public AudioClip firingSound;
    [Range(0.0f, 1.0f)] public float firingSoundVolume = 1.0f;
    public AudioClip travelSound;
    [Range(0.0f, 1.0f)] public float travelSoundVolume = 1.0f;
    public AudioClip hitSound;
    [Range(0.0f, 1.0f)] public float hitSoundVolume = 1.0f;
    public AudioClip dashedIntoSound;
    [Range(0.0f, 1.0f)] public float dashedIntoSoundVolume = 1.0f;
    public AudioClip enhancedAttackedSound;
    [Range(0.0f, 1.0f)] public float enhancedAttackedSoundVolume = 1.0f;
}
