using UnityEngine;
using System.Collections.Generic;
public class EnemyVision : EnemyComponent
{
    [SerializeField, Range(0.0f, 5.0f)] private float visionRadius = 5.0f;
    [SerializeField] private LayerMask detectionLayerMask;
    [SerializeField] private LayerMask lineOfSightLayermask;

    [SerializeField, Range(0.0f, 2.0f)] private float loseTargetTime = 1.0f;
    private void Start()
    {
        if (detectionLayerMask == 0) Debug.LogWarning($"{this.name}'s detectionLayerMask is set to Nothing! Should you set it to something?");
        if (lineOfSightLayermask == 0) Debug.LogWarning($"{this.name}'s lineOfSightLayermask is set to Nothing! Should you set it to something?");
    }
    private void Update()
    {
        if (enemy.isAttacking) return;
        if (!enemy.allowVision || enemy.isBeingPummeled) return;
        if (enemy.target != null) CheckLineOfSight();
        if (enemy.target == null) SearchForTarget();
    }
    private void SearchForTarget()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, visionRadius, detectionLayerMask);
        // No targets within Overlap Circle
        if (collider == null) return;
        RaycastHit2D hit = Physics2D.Linecast(transform.position, collider.transform.position, lineOfSightLayermask);
        // Direct Line of Sight is blocked
        if (!CheckIsTarget(hit.transform, collider.gameObject)) return;
        enemy.AssignTarget(collider.gameObject);
    }

    private float lastSeenTargetTime = -90.0f;
    private void CheckLineOfSight()
    {
        if (enemy.target == null) return;
        RaycastHit2D hit = Physics2D.Linecast(transform.position, enemy.target.transform.position, lineOfSightLayermask);
        if (CheckIsTarget(hit.transform)) // Target can be seen
        {
            lastSeenTargetTime = Time.time;
            return;
        }
        // Target CANNOT be seen
        if (Time.time - lastSeenTargetTime >= loseTargetTime)
        {
            Debug.Log("Lost sight of target");
            enemy.UnassignTarget();
        }
    }
    /// <summary>
    /// Check if input Transform or any of it's Parents is this enemy's Target
    /// </summary>
    /// <param name="input">The Gameobject's Transform to examine</param>
    /// <param name="target">The Gameobject to compare the input to, setting this to null defaults it to enemy.target</param>
    /// <returns>True if input IS or BELONGS TO the Target<br/>False otherwise</returns>
    private bool CheckIsTarget(Transform input, GameObject target = null)
    {
        if (target == null) target = enemy.target;
        if (target == null) return false;
        while (input != null)
        {
            if (input.gameObject == target) return true;
            else input = input.transform.parent;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        if (!enemy.allowVision || enemy.target != null) return;

        Gizmos.color = new Color(Color.green.r, Color.green.g, Color.green.b, 0.2f);
        Gizmos.DrawSphere(transform.position, visionRadius);
    }
}
