using UnityEngine;

[CreateAssetMenu(fileName = "BulletSFX", menuName = "Scriptable Objects/BulletSFX")]
public class BulletSFX : ScriptableObject
{
    public AudioClip firingSound;
    public AudioClip travelSound;
    public AudioClip hitSound;
    public AudioClip dashedIntoSound;
    public AudioClip enhancedAttackedSound;
}
