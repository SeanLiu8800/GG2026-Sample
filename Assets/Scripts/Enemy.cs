using UnityEngine;

public class Enemy : MonoBehaviour
{
    [field : SerializeField] public EnemyHealth health { get; private set; }

    void Awake()
    {
        if (!TryGetComponent<EnemyHealth>(out EnemyHealth _health))
        {
            Debug.LogError($"{this.name} DOES NOT have an EnemyHealth Component!");
            return;
        }

        health = _health;
    }
}
