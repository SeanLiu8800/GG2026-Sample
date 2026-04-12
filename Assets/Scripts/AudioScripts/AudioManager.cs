using UnityEngine;
using System.Collections;
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] private AudioSource audioSource;

    [SerializeField] private AudioSource MusicIntroSource;
    [SerializeField] private AudioSource MusicLoopSource;
    [SerializeField] private AudioSource MusicInterludeSource;
    [SerializeField] private AudioSource MusicOutroSource;

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

        MusicLoopSource.loop = true;
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

    private Soundtrack currSoundtrack;
    public void PlaySoundtrack(Soundtrack soundtrack)
    {
        currSoundtrack = soundtrack;

        double startTime = AudioSettings.dspTime + 0.2;
        double introDuration = 0.0;
        if (soundtrack.intro == null)
        {
            Debug.LogWarning($"{soundtrack.name}'s intro component is NULL!");
            introDuration = 0.0;
        }
        else
        {
            MusicIntroSource.clip = soundtrack.intro;
            introDuration = soundtrack.intro.length;
            MusicIntroSource.PlayScheduled(startTime);
        }

        if (soundtrack.mainLoop == null)
        {
            Debug.LogWarning($"{soundtrack.name}'s mainLoop component is NULL!");
            if (soundtrack.intro != null)
            {
                Debug.LogWarning($"MainLoop falling back to Intro!");
                MusicLoopSource.clip = soundtrack.intro;
                MusicLoopSource.PlayScheduled(startTime + introDuration);
            }
        }
        else
        {
            MusicLoopSource.clip = soundtrack.mainLoop;
            MusicLoopSource.PlayScheduled(startTime + introDuration);
        }
    }
    
}
