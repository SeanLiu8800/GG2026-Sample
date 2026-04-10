using UnityEngine;

public class BulletMovement : BulletComponent
{
    private void Update()
    {
        transform.position = 
            transform.position + (bullet.moveDirection * bullet.moveSpeed * Time.deltaTime);
    }
}
