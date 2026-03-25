using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
public class Turret : MonoBehaviour
{
    [SerializeField] private GameObject bullet;

    private void Update()
    {
        if (Keyboard.current.tKey.wasPressedThisFrame) StartCoroutine(Shoot());
    }

    private IEnumerator Shoot()
    {
        for (int i = 0; i < 5; i ++)
        {
            GameObject currBullet = Instantiate(bullet, transform.position, transform.rotation);
            currBullet.GetComponent<BulletScript>()?.SetLinearVelocity(Vector3.left * 3);
            yield return new WaitForSeconds(0.2f);
        }
        yield break;
    }
}
