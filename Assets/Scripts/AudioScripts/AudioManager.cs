using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource MusicIntroSource;
    [SerializeField] private AudioSource MusicLoopSource;
    [SerializeField] private AudioSource MusicInterludeSource;
    [SerializeField] private AudioSource MusicOutroSource;

    [SerializeField] private List<AudioSource> SFXAudioSourcePool;

    [Header("Volume Variables")]
    [SerializeField, Range(0.0f, 1.0f)] private float sfxVolume = 1.0f;
    [SerializeField, Range(0.0f, 1.0f)] private float musicVolume = 1.0f;
    [SerializeField] private Soundtrack tempSoundtrack;

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
            DontDestroyOnLoad(gameObject);
        }

        MusicLoopSource.loop = true;
        MusicInterludeSource.loop = true;
    }
    private void OnValidate()
    {
        foreach (AudioSource audioSource in SFXAudioSourcePool) audioSource.volume = sfxVolume;

        MusicIntroSource.volume = musicVolume;
        MusicLoopSource.volume = musicVolume;
        MusicInterludeSource.volume = musicVolume;
        MusicOutroSource.volume = musicVolume;
    }
    //private void Update()
    //{
    //    if (Keyboard.current.enterKey.wasPressedThisFrame) PlaySoundtrack(tempSoundtrack);
    //    if (Keyboard.current.minusKey.wasPressedThisFrame) SoundtrackSwitchToInterlude();
    //    if (Keyboard.current.equalsKey.wasPressedThisFrame) SoundtrackSwitchToMain();
    //    if (Keyboard.current.backspaceKey.wasPressedThisFrame) StopSountrack();
    //}

    public void PlaySoundOneShot(AudioClip audioClip, float volume = 1.0f,  float pitch = 1.0f)
    {
        if (AudioClipIsNull(audioClip)) return;

        foreach (AudioSource audioSource in SFXAudioSourcePool)
        {
            if (audioSource.isPlaying) continue;

            audioSource.pitch = pitch;
            audioSource.volume = volume;
            audioSource.clip = audioClip;
            audioSource.Play();
            return;
        }

        SFXAudioSourcePool.Add(this.gameObject.AddComponent<AudioSource>());
        SFXAudioSourcePool[SFXAudioSourcePool.Count - 1].pitch = pitch;
        SFXAudioSourcePool[SFXAudioSourcePool.Count - 1].volume = volume;
        SFXAudioSourcePool[SFXAudioSourcePool.Count - 1].clip = audioClip;
        SFXAudioSourcePool[SFXAudioSourcePool.Count - 1].Play();
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
        if (soundtrack.intro == null) Debug.LogWarning($"{soundtrack.name}'s intro component is NULL!");
        else
        {
            MusicIntroSource.clip = soundtrack.intro;
            MusicIntroSource.volume = musicVolume;
            introDuration = soundtrack.intro.length - 3.0f;
            MusicIntroSource.PlayScheduled(startTime);
        }

        if (soundtrack.mainLoop == null)
        {
            Debug.LogWarning($"{soundtrack.name}'s mainLoop component is NULL!");
            if (soundtrack.intro != null)
            {
                Debug.LogWarning($"MainLoop falling back to Intro!");
                MusicLoopSource.clip = soundtrack.intro;
                MusicLoopSource.volume = musicVolume;
            }
        }
        else MusicLoopSource.clip = soundtrack.mainLoop;

        MusicLoopSource.PlayScheduled(startTime + introDuration);

        if (soundtrack.interlude == null) Debug.LogWarning($"{soundtrack.name}'s interlude component is NULL!");
        else
        {
            MusicInterludeSource.volume = 0.0f;
            MusicInterludeSource.clip = soundtrack.interlude;
            MusicInterludeSource.PlayScheduled(startTime);
        }

        if (soundtrack.outro == null) Debug.LogWarning($"{soundtrack.name}'s outro component is NULL!");
        else
        {
            MusicOutroSource.volume = 0.0f;
            MusicOutroSource.clip = soundtrack.outro;
        }
    }
    public void SoundtrackSwitchToInterlude(float crossfadeDuration = 2.0f)
    {
        List<AudioSource> toMute = new List<AudioSource>();
        toMute.Add(MusicIntroSource);
        toMute.Add(MusicLoopSource);
        List<AudioSource> toUnmute = new List<AudioSource>();
        toUnmute.Add(MusicInterludeSource);

        if (crossfadeCoroutine != null) StopCoroutine(crossfadeCoroutine);
        crossfadeCoroutine = StartCoroutine(Crossfade(toMute, toUnmute, crossfadeDuration));
    }
    public void SoundtrackSwitchToMain(float crossfadeDuration = 2.0f)
    {
        List<AudioSource> toMute = new List<AudioSource>();
        toMute.Add(MusicInterludeSource);
        List<AudioSource> toUnmute = new List<AudioSource>();
        toUnmute.Add(MusicIntroSource);
        toUnmute.Add(MusicLoopSource);

        if (crossfadeCoroutine != null) StopCoroutine(crossfadeCoroutine);
        crossfadeCoroutine = StartCoroutine(Crossfade(toMute, toUnmute, crossfadeDuration));
    }
    public void SoundtrackPlayOutro(float crossfadeDuration = 2.0f)
    {
        if (MusicOutroSource.clip != null) MusicOutroSource.Play();

        List<AudioSource> toMute = new List<AudioSource>();
        toMute.Add(MusicIntroSource);
        toMute.Add(MusicLoopSource);
        toMute.Add(MusicInterludeSource);

        List<AudioSource> toUnmute = new List<AudioSource>();
        toUnmute.Add(MusicOutroSource);

        if (crossfadeCoroutine != null) StopCoroutine(crossfadeCoroutine);
        crossfadeCoroutine = StartCoroutine(Crossfade(toMute, toUnmute, crossfadeDuration));

        MusicIntroSource.SetScheduledEndTime(AudioSettings.dspTime + crossfadeDuration);
        MusicLoopSource.SetScheduledEndTime(AudioSettings.dspTime + crossfadeDuration);
        MusicInterludeSource.SetScheduledEndTime(AudioSettings.dspTime + crossfadeDuration);
    }
    private Coroutine crossfadeCoroutine;
    private IEnumerator Crossfade(List<AudioSource> toMute, List<AudioSource> toUnmute, float crossfadeDuration = 2.0f)
    {
        float startTime = Time.time;
        float progress = (crossfadeDuration <= 0.0f ? 1.0f : (Time.time - startTime) / crossfadeDuration);
        float toMuteStartVolume = toMute[0].volume;
        float toUnmuteStartVolume = toUnmute[0].volume;
        while (progress < 1.0f)
        {
            foreach (AudioSource currAudioSource in toMute) currAudioSource.volume = Mathf.Lerp(toMuteStartVolume, 0.0f, progress);
            foreach (AudioSource currAudioSource in toUnmute) currAudioSource.volume = Mathf.Lerp(toUnmuteStartVolume, musicVolume, progress);
            yield return null;
            progress = (Time.time - startTime) / crossfadeDuration;
        }
        foreach (AudioSource currAudioSource in toMute) currAudioSource.volume = 0.0f;
        foreach (AudioSource currAudioSource in toUnmute) currAudioSource.volume = 1.0f;
    }

    public void StopSountrack()
    {
        MusicIntroSource.Stop();
        MusicLoopSource.Stop();
        MusicInterludeSource.Stop();
        MusicOutroSource.Stop();
    }
}