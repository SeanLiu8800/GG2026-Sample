using UnityEngine;
using System.Collections;
public abstract class GrabCinematicBase : MonoBehaviour
{
    [SerializeField] protected GameObject target;
    [SerializeField] protected GameObject owner;

    [SerializeField, Range(0.0f, 10.0f)] private float timeoutTime = 5.0f;
    public void Initialize(GameObject target, GameObject owner)
    {
        this.target = target;
        this.owner = owner;
    }

    private void Start()
    {
        if (target == null || owner == null)
        {
            Debug.LogError($"Player or Enemy is NULL! Deleting!");
            Destroy(this.gameObject);
            return;
        }
        else
        {
            StartCoroutine(GrabCinematic());
            StartCoroutine(EndCinematic());
        }
    }

    protected abstract IEnumerator GrabCinematic();
    private IEnumerator EndCinematic()
    {
        yield return new WaitForSeconds(timeoutTime);

            // Reenable Player Controls
            // Reenable Enemy

        Destroy(this.gameObject);
    }
}
