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
    public float ImpulseFromDistance(float distanceToTravel)
    {
        float impulse = distanceToTravel * enemy.enemyRigidbody.linearDamping * enemy.enemyRigidbody.mass;
        return impulse;
    }
    /// <summary>
    /// Have this enemy Dash maxDistance towards the target, or to within 1.5f of the target, whichever has the smaller travel distance
    /// </summary>
    /// <param name="maxDistance">The maximum dash distance</param>
    /// <param name="distanceFromTarget">The distance from the target to end the dash</param>
    public void DashToTarget(float maxDistance, float distanceFromTarget = 1.5f)
    {
        float travelDistance = Mathf.Min(
            enemy.distanceToTargetSquared - distanceFromTarget * distanceFromTarget, 
            maxDistance
        );
        float travelImpulse = ImpulseFromDistance(travelDistance);
        enemy.enemyRigidbody.AddForce(enemy.toTargetDirection * travelImpulse, ForceMode2D.Impulse);
    }
    /// <summary>
    /// Movement Behavior that allows movement towards strafeRadius units away from Player, then strafe in a circle
    /// </summary>
    private void ChaseThenStrafe()
    {
        if (!enemy.allowMove) return;
        if (!enemy.canMove || enemy.target == null) return;

        Vector3 toTargetVector = enemy.toTargetVector;
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
