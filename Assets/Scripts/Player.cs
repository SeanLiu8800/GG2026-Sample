using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [field : HideInInspector] public PlayerMovement movement { get; private set; }

}
