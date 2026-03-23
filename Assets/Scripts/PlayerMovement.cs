using UnityEngine;

public class PlayerMovement : PlayerComponent
{
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private Rigidbody2D playerRigidbody;

    [SerializeField, Range(0.0f, 5.0f)] float movementSpeed = 5.0f;
    void FixedUpdate()
    {
        playerRigidbody.linearVelocity = movementInput * movementSpeed;
    }

    private Vector3 movementInput;
    public void MovePlayer(Vector3 input)
    {
        movementInput = input;
    }
}
