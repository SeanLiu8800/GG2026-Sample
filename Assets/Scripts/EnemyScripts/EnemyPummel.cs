using UnityEngine;

public class EnemyPummel : EnemyComponent
{
    [SerializeField] private GameObject latchPoints;

    [field: Header("Pummel Variables")]
    [SerializeField, ReadOnly] private Player pummeler;
    [SerializeField] private bool pummelIndefinitely = false;
    [SerializeField, Range(0.0f, 6.0f)] private float pummelDuration = 5.0f;
    private float pummelStartTime = -99.0f;
    [SerializeField, ReadOnly] private float currentPummelDuration = 0.0f;
    [SerializeField, Range(0, 5)] private int ejectPummelerDamage = 0;
    void OnEnable()
    {
        enemy.enemyEvents.pummelStarts += PummelStarts;
        enemy.enemyEvents.pummelEnds += PummelEnds;
    }
    void OnDisable()
    {
        enemy.enemyEvents.pummelStarts -= PummelStarts;
        enemy.enemyEvents.pummelEnds -= PummelEnds;
    }

    #region ----- Event Functions -----
    protected virtual void PummelStarts(Player player)
    {
        enemy.isBeingPummeled = true;
        pummeler = player;
        pummelStartTime = Time.time;
    }
    protected virtual void PummelEnds()
    {
        enemy.isBeingPummeled = false;
        this.pummeler = null;
    }
    #endregion

    void Update()
    {
        UpdatePummel();
    }

    private void UpdatePummel()
    {
        if (!enemy.isBeingPummeled || pummeler == null) return;
        currentPummelDuration = Time.time - pummelStartTime;
        if (!pummelIndefinitely && currentPummelDuration >= pummelDuration) EjectPummeler();
    }
    private void EjectPummeler()
    {
        if (!enemy.isBeingPummeled || pummeler == null) return;
        pummeler.health.Damage(ejectPummelerDamage);
        pummeler.move.KnockBack(-enemy.toTargetDirection * 10.0f);

        pummeler.pummel.EjectedByPummelTarget();
        enemy.enemyEvents.pummelEnds?.Invoke();
    }

    public Vector3 GetClosestLatchPoint(Vector3 inputPosition)
    {
        float minDistance = float.PositiveInfinity;
        Vector3 returnPoint = transform.position;
        foreach (Transform childTransform in latchPoints.transform)
        {
            float currDistance = Vector3.SqrMagnitude(childTransform.transform.position - inputPosition);
            if (currDistance < minDistance)
            {
                minDistance = currDistance;
                returnPoint = childTransform.position;
            }
        }

        return returnPoint;
    }
}
