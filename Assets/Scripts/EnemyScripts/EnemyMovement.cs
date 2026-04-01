using UnityEngine;

public class EnemyMovement : EnemyComponent
{
    [SerializeField, Range(1.0f, 10.0f)] private float distanceToTarget = 3.5f;
    [SerializeField, Range(0.0f, 10.0f)] private float moveSpeed = 5.0f;
    [SerializeField, Range(1.0f, 10.0f)] private float strafingThreshold = 5.0f;
    void Update()
    {
        MaintainDistance();
    }
    private void MaintainDistance()
    {
        if (enemy.target == null) return;

        Vector3 toTargetVector = enemy.target.transform.position - transform.position;
        Vector3 strafeVector = Quaternion.Euler(new Vector3(0, 0, 90)) * toTargetVector;
        float currDistance = Vector3.Magnitude(toTargetVector);
        if (currDistance > distanceToTarget)
        {
            Vector3 resultVec = toTargetVector + strafeVector;
            enemy.enemyRigidbody.AddForce(resultVec.normalized * moveSpeed);
        }
        else
        {
            Vector3 resultVec = -1.0f * toTargetVector + strafeVector;
            enemy.enemyRigidbody.AddForce(resultVec.normalized * moveSpeed);
        }
    }
}
