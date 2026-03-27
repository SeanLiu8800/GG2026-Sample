using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private AudioSource audioSource;
    public SoundEffects soundEffects { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    
    public void PlaySoundOneShot(AudioClip audioClip)
    {
        if (AudioClipIsNull(audioClip)) return;

        audioSource.PlayOneShot(audioClip);
    }
    private bool AudioClipIsNull(AudioClip audioClip)
    {
        if (audioClip != null) return true;

        Debug.LogError($"{audioClip.name} is null/ unassigned!");
        return false;
    }
}
