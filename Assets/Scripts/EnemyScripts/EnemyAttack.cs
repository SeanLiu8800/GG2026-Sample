using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class EnemyAttack : EnemyComponent
{
    [Header("Attack Variables")]
    [SerializeField] private GameObject attack;
    [SerializeField] private GameObject meleeAttack;

    [SerializeField, Range(0.0f, 5.0f)] private float attackCooldown = 1.0f;

    void Update()
    {
        Attack();
        if (Keyboard.current.tKey.wasPressedThisFrame) StartCoroutine(Shoot());
        if (Keyboard.current.gKey.wasPressedThisFrame) MeleeAttack();
    }

    private void Attack()
    {
        if (!enemy.canAttack || enemy.target == null) return;
        if (Random.Range(0, 2) == 0) StartCoroutine(Shoot());
        else StartCoroutine(MeleeAttack());
    }
    public void AttackCooldown()
    {
        if (attackCooldownCoroutine != null) StopCoroutine(attackCooldownCoroutine);
        attackCooldownCoroutine = StartCoroutine(AttackCooldownCoroutine());
    }
    private Coroutine attackCooldownCoroutine;
    private IEnumerator AttackCooldownCoroutine()
    {
        enemy.canAttack = false;
        float cooldownStartTime = Time.time;
        while (Time.time - cooldownStartTime <= attackCooldown) yield return null;
        enemy.canAttack = true;
        yield break;
    }
    private IEnumerator Shoot()
    {
        enemy.canAttack = false;
        for (int i = 0; i < 5; i++)
        {
            SpawnAttack(
                attack,
                enemy.target,
                (enemy.target.transform.position - transform.position).normalized * 7.0f,
                enemy.target.transform.position - transform.position
            );
            yield return new WaitForSeconds(0.2f);
        }

        AttackCooldown();
        yield break;
    }
    private IEnumerator MeleeAttack()
    {
        enemy.canAttack = false;
        enemy.canMove = false;
        yield return new WaitForSeconds(0.2f);
        enemy.enemyRigidbody.AddForce((enemy.target.transform.position - transform.position).normalized * 50.0f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f);
        AudioManager.Instance.PlaySoundOneShot(AudioManager.Instance.soundEffects.playerAttack);
        SpawnAttack(meleeAttack, enemy.target, default, enemy.target.transform.position - transform.position);
        yield return new WaitForSeconds(0.1f);
        enemy.canMove = true;
        AttackCooldown();
    }
    private void SpawnAttack
    (
        GameObject attack, 
        GameObject target, 
        Vector3 initialLinearVelocity = default(Vector3), 
        Vector3 lookDirection = default(Vector3)
    )
    {
        GameObject currAttack = Instantiate(attack, transform.position, transform.rotation);
        if (!currAttack.TryGetComponent<BulletScript>(out BulletScript currBulletScript))
        {
            Debug.LogWarning($"{this.attack.name} DOES NOT have a BulletScript");
        }
        else
        {
            currBulletScript.Initialize(
                this.gameObject,
                target,
                initialLinearVelocity,
                lookDirection
            );
        }
    }
}
