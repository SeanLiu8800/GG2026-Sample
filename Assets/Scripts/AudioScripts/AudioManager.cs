using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private AudioSource audioSource;
    [field : SerializeField] public SoundEffects soundEffects { get; private set; }
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
        if (audioClip != null) return false;

        Debug.LogError($"This Audio Clip is null/ unassigned!");
        return true;
    }
}
