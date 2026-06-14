using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
        // Cache BulletScripts of this bullet's children
        List<BulletScript> childBullets = GetChildBullets();
        float rotationStartTime = Time.time;
        while (Time.time - rotationStartTime < rotationDuration)
        {
            // Update cache if the number in the cache is different to the number of actual children
            // NOTE: CACHE WON'T BE UPDATED IF THERE IS AN EQUAL NUMBER OF CHILD BULLETS CREATED AND DESTROYED
            // BEFORE THIS COMPARISON IS CHECKED!!!
            if (this.transform.childCount != childBullets.Count) GetChildBullets();

            transform.Rotate(Vector3.forward * rotationRate * Time.deltaTime);
            foreach (BulletScript currBullet in childBullets) currBullet.moveDirection = Quaternion.Euler(0, 0, rotationRate * Time.deltaTime) * currBullet.moveDirection;
            rotationRate += rotationAcceleration * Time.deltaTime;
            yield return null;
        }
        yield break;
    }
    /// <summary>
    /// Gathers the BulletScripts of all child bullets of this bullet
    /// </summary>
    /// <returns>A List of the BulletScripts of all Child Bullets</returns>
    private List<BulletScript> GetChildBullets()
    {
        List<BulletScript> childBullets = new List<BulletScript>();
        foreach (Transform childTransform in this.transform)
        {
            if (!childTransform.TryGetComponent<BulletScript>(out BulletScript currBullet)) continue;
            childBullets.Add(currBullet);
        }

        return childBullets;
    }
}
