using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class EnemyAttack : EnemyComponent
{
    [SerializeField] private GameObject target; 
    [SerializeField] private GameObject attack;
    [SerializeField] private GameObject meleeAttack;
    void OnEnable()
    {
        enemy.enemyEvents.onParried += OnParried;
    }
    void OnDisable()
    {
        enemy.enemyEvents.onParried -= OnParried;
    }
    #region ----- Event Functions -----
    void OnParried(GameObject parrier)
    {
        Vector3 knockbackDirection = (this.transform.position - parrier.transform.position).normalized;
        Debug.Log($"{this.name} WAS PARRIED, Knockback Vector is {knockbackDirection}");
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
                        target, 
                        (target.transform.position - transform.position).normalized * 3
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
                transform.position + (target.transform.position - transform.position).normalized,
                transform.rotation
            );
        if (currMeleeAttack.TryGetComponent<BulletScript>(out BulletScript currBulletScript))
        {
            currBulletScript.Initialize(this.gameObject, target);
        }
    }
}
