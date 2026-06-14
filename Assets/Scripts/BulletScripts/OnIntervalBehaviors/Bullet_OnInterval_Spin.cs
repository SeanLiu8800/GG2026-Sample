using UnityEngine;
using System.Collections;
public class Bullet_OnInterval_Spin : Bullet_OnIntervalBehaviorBase
{
    [Header("Spinning Variables")]
    [SerializeField, Range(0.0f, 360.0f)] private float rotationRate = 30.0f;
    [SerializeField, Range(-360.0f, 360.0f)] private float rotationAcceleration = 0.0f;
    [SerializeField, Range(0.0f, 5.0f)] private float rotationDuration = 1.0f;
    protected override void IntervalAction()
    {
        StartCoroutine(SpinCoroutine());
    }
    private IEnumerator SpinCoroutine()
    {
        float rotationStartTime = Time.time;
        while (Time.time - rotationStartTime < rotationDuration)
        {
            transform.Rotate(Vector3.forward * rotationRate * Time.deltaTime);
            foreach (Transform childTransform in this.transform)
            {
                if (childTransform.TryGetComponent<BulletScript>(out BulletScript currBullet))
                {
                    currBullet.moveDirection = Quaternion.Euler(0, 0, rotationRate * Time.deltaTime) * currBullet.moveDirection;
                }
            }
            rotationRate += rotationAcceleration * Time.deltaTime;
            yield return null;
        }
        yield break;
    }
}
