using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerAttack : PlayerComponent
{
    [SerializeField] private Collider2D attackArea;
    protected override void Awake()
    {
        base.Awake();
    }
    void OnEnable()
    {
        
    }
    void OnDisable()
    {
        
    }
    void Update()
    {
        if (Keyboard.current?.spaceKey.wasReleasedThisFrame == true)
        {
            Debug.Log("ATTACK");
        }
    }
    void FixedUpdate()
    {
        attackArea.transform.rotation = Quaternion.LookRotation(Vector3.forward, player.movement.lastMovementDirection);
    }

    private void Attack(InputAction.CallbackContext context)
    {

    }
}
