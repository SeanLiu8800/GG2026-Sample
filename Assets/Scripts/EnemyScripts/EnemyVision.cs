using UnityEngine;

public class EnemyVision : EnemyComponent
{
    [SerializeField] private Collider2D visionArea;
    [SerializeField] private LayerMask layerToDetect;
    [SerializeField] private LayerMask lineOfSightLayermask;
    private void Start()
    {
        if (enemy.target != null) EnableVision();
        else DisableVision();

        if (layerToDetect == 0) Debug.LogWarning($"{this.name}'s layerMask is set to Nothing! Should you set it to something?");
        if (lineOfSightLayermask == 0) Debug.LogWarning($"{this.name}'s Line of Sight layerMask is set to Nothing! Should you set it to something?");
    }
    private void Update()
    {
        if (enemy.target != null) CheckLineOfSight();
        if (!visionArea.enabled && enemy.target == null) EnableVision();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & layerToDetect) == 0) return;

        RaycastHit2D hit = Physics2D.Linecast(transform.position, collision.transform.position, lineOfSightLayermask);
        if (!CheckIsTarget(hit.transform, collision.gameObject)) return;

        enemy.target = collision.gameObject;
        DisableVision();
    }

    private void EnableVision()
    {
        visionArea.enabled = true;
    }
    private void DisableVision()
    {
        visionArea.enabled = false;
    }

    private void CheckLineOfSight()
    {
        if (enemy.target == null) return;
        RaycastHit2D hit = Physics2D.Linecast(transform.position, enemy.target.transform.position, lineOfSightLayermask);
        if (CheckIsTarget(hit.transform)) return;

        Debug.Log("Lost sight of target");
        enemy.target = null;
    }
    /// <summary>
    /// Check if input Transform or any of it's Parents is this enemy's Target
    /// </summary>
    /// <param name="input">The Gameobject's Transform to examine</param>
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
}
