using UnityEngine;
using System.Collections;
public class Bullet_OnInterval_DirRouletteEmit : Bullet_OnIntervalBehaviorBase
{
    [Header("Emission Variables")]
    [SerializeField] private GameObject bulletToEmit;
    [SerializeField, Range(1, 5)] private int emissionCount = 1;
    [SerializeField] private Vector3 emitDirection = Vector3.up;
    [SerializeField, Range(-720.0f, 720.0f)] private float spinRate = 360f;
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
    }
    protected override void IntervalAction()
    {
        GameObject emittedBullet = 
            Instantiate(
                bulletToEmit, 
                transform.position - emitDirection*5.0f, 
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
                    emitDirection * bullet.initialLinearVelocity.magnitude,
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
            emitDirection = Quaternion.AngleAxis(spinRate * Time.deltaTime, Vector3.forward) * emitDirection;
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.DrawLine(transform.position, transform.position + emitDirection);
    }
}
