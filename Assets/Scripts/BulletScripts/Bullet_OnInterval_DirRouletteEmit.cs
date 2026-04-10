using UnityEngine;
using System.Collections;
public class Bullet_OnInterval_DirRouletteEmit : Bullet_OnIntervalBehaviorBase
{
    public static Vector3 previousDirection = Vector3.up;

    [Header("Emission Variables")]
    [SerializeField] private GameObject bulletToEmit;
    [SerializeField, Range(1.0f, 20.0f)] private float distanceFromTarget = 3.0f;

    [Header("Direction Roulette Variables")]
    [SerializeField] private GameObject arrow;
    [SerializeField, ReadOnly] private Vector3 emitDirection = Vector3.up;
    [Tooltip("How many rotations per second the roulette spins")]
    [SerializeField, Range(-2.0f, 2.0f)] private float spinRate = 1;

    [Header("Arrow Activation Variables")]
    [Tooltip("The time to activate the Spinning Pointer Arrow. Activation occurs activateTimeDiff seconds before initialActionDelay!")]
    [SerializeField, Range(0.0f, 10.0f)] private float activateTimeDiff = 0.25f;
    [SerializeField] private bool stopSpinUponActivation = true;
    protected override void Start()
    {
        if (bulletToEmit == null)
        {
            Debug.LogError($"{this.name}'s Fan Emit Component doesn't have an bullet to Emit! Destroying!");
            Destroy(this);
            return;
        }

        base.Start();
        DetermineInitialDirection();
        StartSpinning();
        DeactivateArrowCollider();
        Invoke(nameof(ActivateArrowCollider), Mathf.Max(initialActionDelay - activateTimeDiff, 0));
    }
    protected override void IntervalAction()
    {
        GameObject emittedBullet = 
            Instantiate(
                bulletToEmit, 
                transform.position - emitDirection * distanceFromTarget, 
                transform.rotation
            );
        if (!emittedBullet.TryGetComponent<BulletScript>(out BulletScript currBulletScript))
            Debug.LogError("Emitted Bullet DOES NOT have an BulletScript Component!");
        else
        {
            currBulletScript.Initialize
                (
                    bullet.owner,
                    bullet.target,
                    emitDirection,
                    emitDirection
                );
        }
    }
    
    private void DetermineInitialDirection()
    {
        emitDirection = Quaternion.AngleAxis(Random.Range(45, 225), Vector3.forward) * previousDirection;
        previousDirection = emitDirection;

        if (arrow != null) arrow.transform.rotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2(emitDirection.y, emitDirection.x) * Mathf.Rad2Deg);
    }

    private void ActivateArrowCollider()
    {
        if (stopSpinUponActivation) StopSpinning();
        if (arrow != null)
        {
            Collider2D arrowCollider = arrow.GetComponentInChildren<Collider2D>();
            if (arrowCollider != null) arrowCollider.enabled = true;
        }
    }
    private void DeactivateArrowCollider()
    {
        if (arrow != null)
        {
            Collider2D arrowCollider = arrow.GetComponentInChildren<Collider2D>();
            if (arrowCollider != null) arrowCollider.enabled = false;
        }
    }

    private void StartSpinning()
    {
        if (SpinCoroutine != null) StopCoroutine(SpinCoroutine);
        SpinCoroutine = StartCoroutine(SpinDirection());
    }
    private void StopSpinning()
    {
        if (SpinCoroutine != null) StopCoroutine(SpinCoroutine);
    }
    private Coroutine SpinCoroutine;
    private IEnumerator SpinDirection()
    {
        while (true)
        {
            emitDirection = Quaternion.AngleAxis(spinRate * 360.0f * Time.deltaTime, Vector3.forward) * emitDirection;
            previousDirection = emitDirection;
            if (arrow != null) arrow.transform.rotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2(emitDirection.y, emitDirection.x) * Mathf.Rad2Deg);
            yield return null;
        }
    }
}
