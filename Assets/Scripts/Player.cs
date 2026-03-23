using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
    }
    void Update()
    {
        MoveCharacter();
    }
    private InputAction moveAction;
    private void MoveCharacter()
    {
        movement.MovePlayer(moveAction.ReadValue<Vector2>().normalized);
    }
}
