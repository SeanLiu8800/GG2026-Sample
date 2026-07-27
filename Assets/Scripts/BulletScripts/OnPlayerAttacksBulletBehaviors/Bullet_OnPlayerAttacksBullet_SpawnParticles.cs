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

        Instantiate(particles, spawnPosition, particles.transform.rotation);
    }
}
