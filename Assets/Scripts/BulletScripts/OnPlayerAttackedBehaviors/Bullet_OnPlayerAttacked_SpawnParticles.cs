using UnityEngine;

public class Bullet_OnPlayerAttacked_SpawnParticles : Bullet_OnPlayerAttacked_BehaviorBase
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
    protected override void OnPlayerAttackedBehavior(Player player)
    {
        Vector3 spawnPosition = Vector3.Lerp(player.transform.position, this.transform.position, 0.5f);
        Vector3 spawnDirection = player.transform.position - this.transform.position;
        float angle = Mathf.Atan2(spawnDirection.y, spawnDirection.x) * Mathf.Rad2Deg;

        // turn the angle clockwise/ counterclockwise so that it's kinda perpendicular to the clash
        angle = angle + (Random.Range(70.0f, 110.0f) * (Random.Range(0, 2) == 0 ? 1 : -1));
        Instantiate(particles, spawnPosition, Quaternion.AngleAxis(angle, Vector3.forward));
    }
}
