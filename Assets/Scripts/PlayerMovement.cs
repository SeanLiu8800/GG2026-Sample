using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : PlayerComponent
{
    [SerializeField] private Collider2D playerCollider;
    [SerializeField] private Rigidbody2D playerRigidbody;

    private InputAction moveAction;
    [SerializeField, Range(0.0f, 10.0f)] float moveSpeed = 5.0f;
    private Vector3 lastmovementDirection = Vector3.up;
    private Vector3 movementInput;

    private bool isDashing = false;
    private float dashStartTime = 0.0f;
    private Vector3 currDashVelocity;
    [SerializeField, ReadOnly] private float currentDashTime = 0.0f;
    [SerializeField, Range(0.0f, 1.5f)] private float dashTime = 2.0f;
    private Vector3 dashDirection;
    void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
    }
    void Update()
    {
        MoveCharacter();
        if (Keyboard.current.spaceKey.wasPressedThisFrame) StartDash();
        else if (Keyboard.current.spaceKey.wasReleasedThisFrame) StopDash();
    }
    void FixedUpdate()
    {
        if (!isDashing) playerRigidbody.linearVelocity = movementInput * moveSpeed;
        if (isDashing) Dash();
    }
    private void MoveCharacter()
    {
        movementInput = moveAction.ReadValue<Vector2>();
        if (movementInput != Vector3.zero) lastmovementDirection = movementInput;
    }

    private void StartDash()
    {
        isDashing = true;
        dashStartTime = Time.time;
        dashDirection = (movementInput == Vector3.zero) ? lastmovementDirection : movementInput;
        currDashVelocity = dashDirection * 20;
    }
    [Range(0.0f, 5.0f)] public float approach = 3.0f;
    private void Dash()
    {
        currentDashTime = Time.time - dashStartTime;
        if (currentDashTime >= dashTime)
        {
            StopDash();
            return;
        }
        float coef = currentDashTime / dashTime;

        playerRigidbody.linearVelocity = currDashVelocity;
        currDashVelocity = Vector3.Lerp(currDashVelocity, Vector3.zero, Time.deltaTime * approach);
    }
    private void StopDash()
    {
        isDashing = false;
    }
}
