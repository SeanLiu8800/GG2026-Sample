using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerMovement movement;
    void Update()
    {
        MoveCharacter();
    }
    private void MoveCharacter()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        movement.MovePlayer((new Vector3(moveHorizontal, 0.0f, moveVertical)).normalized);
    }
}
