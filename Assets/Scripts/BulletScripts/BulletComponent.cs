using UnityEngine;

public abstract class BulletComponent : MonoBehaviour
{
    public BulletScript bullet { get; private set; }
    
    protected virtual void Awake()
    {
        if (!TryGetComponent<BulletScript>(out BulletScript _bullet))
            Debug.LogError($"{this.name} DOES NOT have a BulletScript Component!");

        bullet = _bullet;
    }
}
