using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;
public class SlowdownManager : MonoBehaviour
{
    public static SlowdownManager Instance;
    [SerializeField] private float globalTimescale = 1.0f;
    [SerializeField] private float actionSlowdownTimescale = 0.01f;

    public bool isDebugging = false;
    [Range(0.0f, 1.0f)] public float easeInDuration = 0.1f;
    [Range(0.0f, 1.0f)] public float slowdownDuration = 0.1f;
    [Range(0.0f, 1.0f)] public float easeOutDuration = 0.1f;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Another SlowdownManager has tried to Initialize! It has been Deleted!");
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Update()
    {
        if (isDebugging && Keyboard.current.rightShiftKey.wasPressedThisFrame) ActionSlowdown(easeInDuration, slowdownDuration, easeOutDuration);
    }
    /// <summary>
    ///     Slows down game to actionSlowdownTimescale (Described in the SlowdownManager) in easeInDuration,<br/>
    ///     maintain it for slowDownDuration, the restore it in easeOutDuration
    /// </summary>
    /// <param name="easeInDuration">Time it takes to reach actionSlowdownTimescale</param>
    /// <param name="slowDownDuration">Time to stay in actionSlowdownTimescale</param>
    /// <param name="easeOutDuration">Time to return to globalTimescale</param>
    public void ActionSlowdown(float easeInDuration = 0.01f, float slowdownDuration = 0.0f, float easeOutDuration = 0.3f)
    {
        StopAllCoroutines();
        StartCoroutine(ActionSlowdownCoroutine(easeInDuration, slowdownDuration, easeOutDuration));
    }
    public void StopSlowdown()
    {
        StopAllCoroutines(); //STOP ALL COROUTINES IS STOPPING EASE TIMESCALE BELOW!
        StartCoroutine(EaseTimescale((x) => x, globalTimescale, 0.0f));
    }
    /// <summary>
    /// Changes the game's Timescale to timescaleTarget in easeDuration seconds<br/>
    /// NOTE: Negative timescaleTarget restores the game's Timescale to the original timescale<br/>
    /// </summary>
    /// <param name="timescaleTarget">Target timescale</param>
    /// <param name="easeDuration">Timescale ease-to duration</param>
    public void ChangeTimescale(float timescaleTarget = 1.0f, float easeDuration = 0.5f)
    {
        if (timescaleTarget == 0.0f) return;
        else if (timescaleTarget < 0.0f) timescaleTarget = globalTimescale;
        StopAllCoroutines();
        this.globalTimescale = timescaleTarget;
        StartCoroutine(EaseTimescale((x) => x, timescaleTarget, easeDuration));
    }
    /// <summary>A Coroutine that Slows down the Game for duration Seconds</summary>
    /// <param name="easeInDuration">Time it takes to reach actionSlowdownTimescale</param>
    /// <param name="slowdownDuration">Time to stay in actionSlowdownTimescale</param>
    /// <param name="easeOutDuration">Time to return to globalTimescale</param>
    private IEnumerator ActionSlowdownCoroutine(float easeInDuration = 0.01f, float slowdownDuration = 0.0f, float easeOutDuration = 0.3f)
    {
        yield return StartCoroutine(EaseTimescale((x) => x, actionSlowdownTimescale, easeInDuration));
        yield return new WaitForSecondsRealtime(slowdownDuration);
        yield return StartCoroutine(EaseTimescale((x) => (-x * x) + 1, globalTimescale, easeOutDuration));
    }
    private IEnumerator EaseTimescale(Func<float, float> easeFunction, float timescaleTarget = 1.0f, float easeDuration = 0.5f)
    {
        if (timescaleTarget <= 0.0f) yield break;

        float startTime = Time.unscaledTime;
        // Ease current unscaled timescale to timescaleTarget
        float currentTimescale = Time.timeScale;
        while (Time.unscaledTime - startTime < easeDuration)
        {
            float coefficient = (Time.unscaledTime - startTime) / easeDuration;
            Time.timeScale = Mathf.Lerp(currentTimescale, timescaleTarget, coefficient * coefficient);
            yield return null;
        }
        Time.timeScale = timescaleTarget;
    }
}
