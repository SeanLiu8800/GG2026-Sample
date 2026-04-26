using UnityEngine;

public class Bullet_OnDamage_PlayGrabCinematic : Bullet_OnDamage_BehaviorBase
{
    [SerializeField] private GameObject grabCinematic;
    private void Start()
    {
        if (grabCinematic == null)
        {
            Debug.LogError("Grab Cinematic is NULL, Set it to something next time! Deleting this Component!");
            Destroy(this);
            return;
        }
        if (!grabCinematic.TryGetComponent<GrabCinematicBase>(out GrabCinematicBase cinematic))
        {
            Debug.LogError("Grab Cinematic is NOT a GrabCinematic!");
            Destroy(this);
            return;
        }
    }
    protected override void OnDamage(GameObject hitObject)
    {
        if (!hitObject.TryGetComponent<Player>(out Player player)){}
        else Instantiate(grabCinematic.gameObject).GetComponent<GrabCinematicBase>().Initialize(bullet.target, bullet.owner);
    }
}
