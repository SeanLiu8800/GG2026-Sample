using UnityEngine;

public class EnemyMovement : EnemyComponent
{
    [SerializeField, Range(1.0f, 3.0f)] private float distanceToTarget = 2.0f;
    [SerializeField, ReadOnly] private Vector3 movementDirection = Vector3.zero;
    [SerializeField, Range(0.0f, 20.0f)] private float closingDistance = 10.0f;
    void Update()
    {
        MaintainDistance();
    }
    private void MaintainDistance()
    {
        if (enemy.target == null) return;

        Vector3 toTargetVector = enemy.target.transform.position - transform.position;
        float currDistance = Vector3.Magnitude(toTargetVector);
        if (currDistance > distanceToTarget)
        {
            movementDirection += toTargetVector * Time.deltaTime;
        }
        else
        {
            movementDirection -= toTargetVector * Time.deltaTime;
        }

        enemy.enemyRigidbody.AddForce(movementDirection * closingDistance);
    }
}
