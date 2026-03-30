using UnityEngine;

public class EnemyParry : EnemyComponent
{
    [Header("Parry Variables")]
    [SerializeField, Range(0.0f, 5.0f)] private float parryProgress = 0;
    [SerializeField, Range(0.0f, 1.0f)] private float parryDecayRate = 0.0f;
    [SerializeField, Range(1.0f, 5.0f)] private float parryTarget = 3.0f;

    [Header("Parry Stun Variables")]
    private float parryStunStartTime = -99.0f;
    [SerializeField, Range(0.0f, 5.0f)] private float parryStunDuration = 3.0f;

    void OnEnable()
    {
        enemy.enemyEvents.onParried += OnParried;
        enemy.enemyEvents.parryStunStarts += ParryStunStarts;
        enemy.enemyEvents.parryStunEnds += ParryStunEnds;

        enemy.enemyEvents.pummelStarts += PummelStarts;
    }
    void OnDisable()
    {
        enemy.enemyEvents.onParried -= OnParried;
        enemy.enemyEvents.parryStunStarts -= ParryStunStarts;
        enemy.enemyEvents.parryStunEnds -= ParryStunEnds;

        enemy.enemyEvents.pummelStarts -= PummelStarts;
    }
    #region ----- Event Functions -----
    protected void OnParried(GameObject parrier)
    {
        if (enemy.isParryStunned) return;

        parryProgress += 1.0f;
        Vector3 knockbackDirection = (this.transform.position - parrier.transform.position).normalized;
        enemy.enemyRigidbody.AddForce(knockbackDirection * 5.0f, ForceMode2D.Impulse);
        Debug.Log($"{this.name} WAS PARRIED, Knockback Vector is {knockbackDirection}");
        if (parryProgress >= parryTarget) enemy.enemyEvents.parryStunStarts?.Invoke();
    }
    protected void ParryStunStarts()
    {
        enemy.isParryStunned = true;
        parryProgress = 0;
        parryStunStartTime = Time.time;
        enemy.spriteRenderer.SetAlpha(0.5f);
    }
    protected void ParryStunEnds()
    {
        enemy.isParryStunned = false;
        enemy.spriteRenderer.SetAlpha(1.0f);
    }
    protected void PummelStarts(Player player)
    {
        ParryStunEnds();
    }
    #endregion
    void Update()
    {
        DecayParryProgress();
        UpdateParryStun();
    }

    private void DecayParryProgress()
    {
        parryProgress -= (parryDecayRate * Time.deltaTime);
    }
    private void UpdateParryStun()
    {
        if (!enemy.isParryStunned || Time.time - parryStunStartTime < parryStunDuration) return;

        enemy.enemyEvents.parryStunEnds?.Invoke();
    }
}
