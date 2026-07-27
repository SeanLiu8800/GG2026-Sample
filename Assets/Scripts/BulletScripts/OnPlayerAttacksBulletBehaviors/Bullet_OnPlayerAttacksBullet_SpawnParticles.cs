using UnityEngine;

public class Bullet_OnPlayerAttacksBullet_SpawnParticles : Bullet_OnPlayerAttacksBullet_BehaviorBase
{
    [Header("Particle Variables")]
    [SerializeField] private ParticleSystem particles;
    private void Start()
    {
        if (particles == null)
        {
            Debug.LogError($"{this.name}'s Bullet_OnPlayerAttacksBullet_SpawnParticles Component DOES NOT have a Particle System! Disabling!");
            this.enabled = false;
            return;
        }
    }
    protected override void OnPlayerAttacksBullet(BulletScript bullet)
    {
        Vector3 spawnPosition = Vector3.Lerp(bullet.transform.position, this.transform.position, 0.5f);
        Vector3 spawnDirection = bullet.transform.position - this.transform.position;
        float angle = Mathf.Atan2(spawnDirection.y, spawnDirection.x) * Mathf.Rad2Deg;
        angle = angle + (Random.Range(70.0f, 110.0f) * (Random.Range(0, 2) == 0 ? 1 : -1));
        Instantiate(particles, spawnPosition, Quaternion.AngleAxis(angle, Vector3.forward));
    }
}
