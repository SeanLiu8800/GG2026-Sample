using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
        double introDuration = 0.0; ;
        if (soundtrack.intro == null) Debug.LogWarning($"{soundtrack.name}'s intro component is NULL!");
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
            }
        }
        else MusicLoopSource.clip = soundtrack.mainLoop;

        MusicLoopSource.PlayScheduled(startTime + introDuration);
    }
    public void SoundtrackSwitchToInterlude(float crossfadeDuration = 1.0f)
    {
        List<AudioSource> toMute = new List<AudioSource>();
        toMute.Add(MusicIntroSource);
        toMute.Add(MusicLoopSource);
        List<AudioSource> toUnmute = new List<AudioSource>();
        toUnmute.Add(MusicInterludeSource);

        if (crossfadeCoroutine != null) StopCoroutine(crossfadeCoroutine);
        crossfadeCoroutine = StartCoroutine(Crossfade(toMute, toUnmute, crossfadeDuration));
    }
    public void SoundtrackSwitchToMain(float crossfadeDuration = 1.0f)
    {
        List<AudioSource> toMute = new List<AudioSource>();
        toMute.Add(MusicInterludeSource);
        List<AudioSource> toUnmute = new List<AudioSource>();
        toUnmute.Add(MusicIntroSource);
        toUnmute.Add(MusicLoopSource);

        if (crossfadeCoroutine != null) StopCoroutine(crossfadeCoroutine);
        crossfadeCoroutine = StartCoroutine(Crossfade(toMute, toUnmute, crossfadeDuration));
    }
    private Coroutine crossfadeCoroutine;
    private IEnumerator Crossfade(List<AudioSource> toMute, List<AudioSource> toUnmute, float crossfadeDuration = 1.0f)
    {
        float startTime = Time.time;
        float progress = Time.time - startTime / crossfadeDuration;
        while (progress < 1.0)
        {
            foreach (AudioSource currAudioSource in toMute) currAudioSource.volume = Mathf.Lerp(1.0f, 0.0f, progress);
            foreach (AudioSource currAudioSource in toUnmute) currAudioSource.volume = Mathf.Lerp(0.0f, 1.0f, progress);
            yield return null;
        }
        foreach (AudioSource currAudioSource in toMute) currAudioSource.volume = 0.0f;
        foreach (AudioSource currAudioSource in toUnmute) currAudioSource.volume = 1.0f;
    }
}
