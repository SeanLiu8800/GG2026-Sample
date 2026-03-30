using UnityEngine;

public class EnemyPummel : EnemyComponent
{
    [SerializeField] private GameObject latchPoints;

    [field : Header("Pummel Variables")]
    [SerializeField, ReadOnly] private Player pummeler;
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
        if (!enemy.isBeingPummeled) return;
        currentPummelDuration = Time.time - pummelStartTime;
        if (currentPummelDuration >= pummelDuration) EjectPummeler();
    }
    private void EjectPummeler()
    {
        pummeler.health.Damage(1);
        Vector3 direction = (pummeler.transform.position - transform.position).normalized;
        pummeler.movement.AddImpulse(direction * 10.0f);

        pummeler.pummel.EjectedByPummelTarget();
        enemy.enemyEvents.pummelEnds?.Invoke();
    }

    public Vector3 GetLeftLatchPointPosition()
    {
        if (latchPoints == null || latchPoints.transform.GetChild(0) == null) return transform.position;
        return latchPoints.transform.GetChild(0).position;
    }
    public Vector3 GetRightLatchPointPosition()
    {
        if (latchPoints == null || latchPoints.transform.GetChild(1) == null) return transform.position;
        return latchPoints.transform.GetChild(1).position;
    }
}
