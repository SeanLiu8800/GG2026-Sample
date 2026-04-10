using UnityEngine;
using System.Collections;
public class Bullet_OnInterval_DirRouletteEmit : Bullet_OnIntervalBehaviorBase
{
    [Header("Emission Variables")]
    [SerializeField] private GameObject bulletToEmit;
    [SerializeField, Range(1, 5)] private int emissionCount = 1;
    [SerializeField, Range(1.0f, 20.0f)] private float distanceFromTarget = 3.0f;

    [Header("Direction Roulette Variables")]
    [SerializeField] private GameObject arrow;
    [SerializeField, ReadOnly] private Vector3 emitDirection = Vector3.up;
    [SerializeField, Range(-2.0f, 2.0f)] private float spinRate = 1;
    [SerializeField, Range(0.0f, 10.0f)] private float stopSpinTime = 0.75f;
    protected override void Start()
    {
        if (bulletToEmit == null)
        {
            Debug.LogError($"{this.name}'s Fan Emit Component doesn't have an bullet to Emit! Destroying!");
            Destroy(this);
            return;
        }
        if (emissionCount <= 0) Debug.LogError($"{this.name}'s emissionCount is 0 or Neagtive! It won't Emit!");

        base.Start();
        emitDirection = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.forward) * emitDirection;
        StartSpinning();
        Invoke(nameof(StopSpinning), stopSpinTime);
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
            if (arrow != null) arrow.transform.rotation = Quaternion.Euler(0.0f, 0.0f, Mathf.Atan2(emitDirection.y, emitDirection.x) * Mathf.Rad2Deg);
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.DrawLine(transform.position, transform.position + emitDirection);
    }
}
