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
        }
        if (!grabCinematic.TryGetComponent<GrabCinematicBase>(out GrabCinematicBase cinematic))
        {
            Debug.LogError("Grab Cinematic is NOT a GrabCinematic!");
            Destroy(this);
        }
    }
    protected override void OnDamage(GameObject hitObject)
    {
        if (!hitObject.TryGetComponent<Player>(out Player player))
        {
            Debug.Log("Object is NOT a Player!");
        }
        else
        {
            Debug.Log("Object IS a Player!");
            Instantiate(grabCinematic.gameObject).GetComponent<GrabCinematicBase>().Initialize(bullet.target, bullet.owner);
        }
    }
}
