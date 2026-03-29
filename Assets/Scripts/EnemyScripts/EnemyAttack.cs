using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
public class EnemyAttack : EnemyComponent
{
    [SerializeField] private GameObject attack;
    [SerializeField] private GameObject meleeAttack;

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
