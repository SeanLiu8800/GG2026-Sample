using UnityEngine;
using UnityEngine.InputSystem;
public class EnemyAttack : EnemyComponent
{
    [SerializeField] private GameObject target; 
    [SerializeField] private GameObject meleeAttack;
    void Update()
    {
        if (Keyboard.current.gKey.wasPressedThisFrame) MeleeAttack();
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
