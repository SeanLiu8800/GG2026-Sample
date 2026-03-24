using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private AudioSource audioSource;
    [field: SerializeField] public AudioClip dashSoundEffect { get; private set; }
    [field: SerializeField] public AudioClip perfectDashSoundEffect { get; private set; }
    [field: SerializeField] public AudioClip imperfectDashSoundEffect { get; private set; }
    [field: SerializeField] public AudioClip dashFailsSoundEffect { get; private set; }
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
        audioSource.PlayOneShot(audioClip);
    }
}
