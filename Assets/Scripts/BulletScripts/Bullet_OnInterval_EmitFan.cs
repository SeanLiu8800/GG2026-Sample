using UnityEngine;
using System.Collections;

public class Bullet_OnInterval_EmitFan : Bullet_OnIntervalBehaviorBase
{
    [Header("Fan Emit Variables")]
    [SerializeField] [Range(0, 360)] private int fanEmissionRadius = 120;
    [SerializeField] [Range(1, 18)] private int emissionCount = 3;
    [SerializeField] private bool spawnClockwise = true;
    [SerializeField] [Range(0, 2)] private float emissionDelay = 0.2f;

    [SerializeField] private GameObject bulletToEmit;
    protected override void Start()
    {
        if (bulletToEmit == null)
        {
            Debug.LogError($"{this.name}'s Fan Emit Component doesn't have an bullet to Emit!");
            return;
        }
        base.Start();
    }
    protected override void IntervalAction()
    {
        StartCoroutine(EmitFan());
    }
    private IEnumerator EmitFan()
    {
        float startingDegree = 0.0f;
        float degreeDifference = 0.0f;
        Vector2 spawnVelocity =
            bullet.bulletRigidbody.linearVelocity == Vector2.zero ? bullet.initialLinearVelocity : bullet.bulletRigidbody.linearVelocity;
        if (emissionCount > 1)
        {
            startingDegree = (spawnClockwise ? 1 : -1) * fanEmissionRadius / 2.0f;
            degreeDifference = (spawnClockwise ? -1 : 1) * fanEmissionRadius / (emissionCount - 1);
        }
        spawnVelocity = Quaternion.Euler(0, 0, startingDegree) * spawnVelocity;
        for (int i = 0; i < emissionCount; i++)
        {
            GameObject emittedBullet = Instantiate(bulletToEmit, this.transform.position, this.transform.rotation);
            if (!emittedBullet.TryGetComponent<BulletScript>(out BulletScript currBulletScript))
                Debug.LogError("Emitted Bullet DOES NOT have an BulletScript Component!");
            else
            {
                currBulletScript.Initialize
                    (
                        bullet.owner,
                        bullet.target,
                        spawnVelocity,
                        spawnVelocity
                    );
            }
            spawnVelocity = Quaternion.Euler(0, 0, degreeDifference) * spawnVelocity;
            yield return new WaitForSeconds(emissionDelay);
        }
    }
}
