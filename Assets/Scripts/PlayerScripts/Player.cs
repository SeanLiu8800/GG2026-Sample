using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [field : SerializeField] public PlayerMovement movement { get; private set; }
    [field : SerializeField] public PlayerAttack attack { get; private set; }
}
