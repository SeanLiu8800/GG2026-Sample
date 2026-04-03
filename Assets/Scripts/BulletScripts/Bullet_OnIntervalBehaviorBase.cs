using UnityEngine;
using System.Collections;

public abstract class Bullet_OnIntervalBehaviorBase : BulletComponent
{
    [Header("Interval Action Variables")]
    [SerializeField] [Range(0.0f, 10.0f)] protected float initialActionDelay = 0.2f;
    [SerializeField] [Range(1, 10)] protected int actionCount = 1;
    [SerializeField] [Range(0.0f, 10.0f)] protected float actionInterval = 0.2f;
    protected virtual void Start()
    {
        if (actionCount <= 0)
        {
            Debug.LogError($"{this.name}'s Interval actionCount is 0 or Negative, nothing will happen!");
            return;
        }
        StartCoroutine(IntervalCoroutine());
    }

    protected abstract void IntervalAction();
    /// <summary>
    ///     A Coroutine that, after [initialActionDelay] seconds, Begins IntervalAction()<br/>
    ///     (To be implemented in an inherited class) [actionCount] times with a [actionInterval]<br/>
    ///     seconds delay in between<br/>
    /// </summary>
    protected IEnumerator IntervalCoroutine()
    {
        yield return new WaitForSeconds(initialActionDelay);
        while (actionCount > 0)
        {
            IntervalAction();
            actionCount = actionCount - 1;
            if (actionCount <= 0) break;

            yield return new WaitForSeconds(actionInterval);
        }
    }
}
