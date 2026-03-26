using UnityEngine;

public abstract class EnemyComponent : MonoBehaviour
{
    public Enemy enemy { get; private set; }
    protected virtual void Awake()
    {
        if (!TryGetComponent<Enemy>(out Enemy _enemy))
        {
            Debug.LogError($"{this.name} DOES NOT have an Enemy Component!");
            return;
        }

        enemy = _enemy;
    }
}
