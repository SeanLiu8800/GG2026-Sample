using UnityEngine;
using System.Collections;
public class Bullet_OnInterval_EmitRadial : Bullet_OnIntervalBehaviorBase
{
    [Header("Radial Emit Variables")]
    [SerializeField] [Range(1, 18)] private int emissionCount = 3;
    [SerializeField] private bool spawnClockwise = true;
    [SerializeField] [Range(0, 2)] private float emissionDelay = 0.2f;

    [SerializeField] private GameObject bulletToEmit;
    protected override void Start()
    {
        if (bulletToEmit == null)
        {
            Debug.LogError($"{this.name}'s Fan Emit Component doesn't have an bullet to Emit! Removing!");
            Destroy(this);
            return;
        }
        if (emissionCount <= 0) Debug.LogError($"{this.name}'s emissionCount is 0 or Neagtive! It won't Emit!");

        base.Start();
    }
    protected override void IntervalAction()
    {
        StartCoroutine(EmitFan());
    }
    private IEnumerator EmitFan()
    {
        float degreeDifference = (spawnClockwise ? -1 : 1) * 360.0f / emissionCount;
        Vector2 spawnDirection = bullet.moveDirection;

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
                        spawnDirection,
                        spawnDirection
                    );
            }
            spawnDirection = Quaternion.Euler(0, 0, degreeDifference) * spawnDirection;
            yield return new WaitForSeconds(emissionDelay);
        }
    }
}
