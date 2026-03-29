using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class EnemyAttack : EnemyComponent
{
    [SerializeField] private GameObject attack;
    [SerializeField] private GameObject meleeAttack;

    [Header("Parry Variables")]
    [SerializeField] private float parryCount = 0;
    [SerializeField, Range(1, 5)] private float parryTarget = 3.0f;
    void OnEnable()
    {
        enemy.enemyEvents.onParried += OnParried;
        enemy.enemyEvents.onParryStun += OnParryStun;
    }
    void OnDisable()
    {
        enemy.enemyEvents.onParried -= OnParried;
        enemy.enemyEvents.onParryStun -= OnParryStun;
    }
    #region ----- Event Functions -----
    void OnParried(GameObject parrier)
    {
        parryCount += 1;
        Vector3 knockbackDirection = (this.transform.position - parrier.transform.position).normalized; 
        Debug.Log($"{this.name} WAS PARRIED, Knockback Vector is {knockbackDirection}");
        if (parryCount >= parryTarget) enemy.enemyEvents.onParryStun?.Invoke();
    }
    void OnParryStun()
    {
        parryCount = 0;
        Debug.Log($"{this.name} Suffers Parry Stun!");
    }
    #endregion
    void Update()
    {
        if (Keyboard.current.tKey.wasPressedThisFrame) StartCoroutine(Shoot());
        if (Keyboard.current.gKey.wasPressedThisFrame) MeleeAttack();
    }

    private IEnumerator Shoot()
    {
        for (int i = 0; i < 5; i++)
        {
            GameObject currBullet = Instantiate(attack, transform.position, transform.rotation);
            if (currBullet.TryGetComponent<BulletScript>(out BulletScript currBulletScript))
            {
                currBulletScript.Initialize
                    (
                        this.gameObject, 
                        enemy.target, 
                        (enemy.target.transform.position - transform.position).normalized * 3
                    );
            }
            yield return new WaitForSeconds(0.2f);
        }
        yield break;
    }
    private void MeleeAttack()
    {
        GameObject currMeleeAttack = Instantiate
            (
                meleeAttack,
                transform.position + (enemy.target.transform.position - transform.position).normalized,
                transform.rotation
            );
        if (currMeleeAttack.TryGetComponent<BulletScript>(out BulletScript currBulletScript))
        {
            currBulletScript.Initialize(this.gameObject, enemy.target);
        }
    }
}
