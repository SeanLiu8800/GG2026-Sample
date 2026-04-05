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
        ChaseThenStrafe();
    }
    
    /// <summary>
    /// Returns the estimated distance traveled from a given impulseStrength<br/>
    /// This assumes traveling will be unimpeded, except by Linear Drag
    /// </summary>
    /// <param name="impulseStrength">Strength of Impulse</param>
    /// <returns>The predicted Distance traveled</returns>
    public float DistanceFromImpulse(float impulseStrength)
    {
        float v0 = impulseStrength / enemy.enemyRigidbody.mass;
        return v0 / enemy.enemyRigidbody.linearDamping;
    }
    /// <summary>
    /// Movement Behavior that allows movement towards strafeRadius units away from Player, then strafe in a circle
    /// </summary>
    private void ChaseThenStrafe()
    {
        if (!enemy.allowMove) return;
        if (!enemy.canMove || enemy.target == null) return;

        Vector3 toTargetVector = enemy.target.transform.position - transform.position;
        Vector3 strafeVector = Quaternion.Euler(new Vector3(0, 0, (strafeClockwise ? 1 : -1) * 90)) * toTargetVector;
        float currDistance = toTargetVector.magnitude;
        float diff = Mathf.Abs(currDistance - strafeRadius);
        float ratio = Mathf.Clamp(diff / strafingThreshold, 0.0f, 0.95f);
        int towardsTarget = (currDistance > strafeRadius ? 1 : -1);

        Vector3 resultVec = ratio * towardsTarget * toTargetVector + (1 - ratio) * strafeVector;
        float moveSpeed = ratio * normalMoveSpeed + (1 - ratio) * strafeMoveSpeed;
        enemy.enemyRigidbody.AddForce(resultVec.normalized * moveSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        strafeClockwise = !strafeClockwise;
    }
}
