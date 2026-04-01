using UnityEngine;

public class EnemyMovement : EnemyComponent
{
    [SerializeField, Range(1.0f, 10.0f)] private float strafeRadius = 3.5f;
    [SerializeField, Range(0.0f, 10.0f)] private float normalMoveSpeed = 3.5f;
    [SerializeField, Range(0.0f, 10.0f)] private float strafeMoveSpeed = 1.0f;
    [SerializeField] private bool strafeClockwise = true;
    [Tooltip("How many units within strafeRadius in which strafing starts occuring")]
    [SerializeField, Range(0.0f, 5.0f)] private float strafingThreshold = 2.0f;
    void Update()
    {
        MaintainDistance();
    }
    
    /// <summary>
    /// Movement Behavior that allows movement towards strafeRadius units away from Player, then strafe in a circle
    /// </summary>
    private void MaintainDistance()
    {
        if (enemy.target == null) return;

        Vector3 toTargetVector = enemy.target.transform.position - transform.position;
        Vector3 strafeVector = Quaternion.Euler(new Vector3(0, 0, (strafeClockwise ? 1 : -1) * 90)) * toTargetVector;
        float currDistance = toTargetVector.magnitude;
        float diff = Mathf.Abs(currDistance - strafeRadius);
        float ratio = Mathf.Clamp(diff / strafingThreshold, 0.0f, 0.95f);
        // Go towards Player
        if (currDistance > strafeRadius)
        {
            Vector3 resultVec = ratio * toTargetVector + (1 - ratio) * strafeVector;
            float moveSpeed = ratio * normalMoveSpeed + (1 - ratio) * strafeMoveSpeed;
            enemy.enemyRigidbody.AddForce(resultVec.normalized * moveSpeed);
        }
        // Go away from Player
        else
        {
            Vector3 resultVec = ratio * -toTargetVector + (1 - ratio) * strafeVector;
            float moveSpeed = ratio * normalMoveSpeed + (1 - ratio) * strafeMoveSpeed;
            enemy.enemyRigidbody.AddForce(resultVec.normalized * moveSpeed);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        strafeClockwise = !strafeClockwise;
    }
}
