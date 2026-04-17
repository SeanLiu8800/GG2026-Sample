using UnityEngine;
using UnityEngine.InputSystem;
public class RoomDebugMenu : MonoBehaviour
{
    private bool currState = false;
    private void Start()
    {
        SetActive(currState);
    }
    private void Update()
    {
        if (Keyboard.current.backquoteKey.wasPressedThisFrame) Toggle();
    }

    public void StartRoom()
    {
        GameManager.Instance.currRoom.roomEvents.roomStarts?.Invoke();
    }
    public void EndRoom()
    {
        GameManager.Instance.currRoom.roomEvents.roomEnds?.Invoke();
    }
    public void KillAllEnemies()
    {
        GameManager.Instance.currRoom.waveSpawner.KillCurrentEnemies();
    }
    public void ResetGame()
    {
        GameManager.Instance.StartGame();
    }

    private void Toggle()
    {
        currState = !currState;
        SetActive(currState);
    }
    private void SetActive(bool input)
    {
        foreach (Transform currTransform in transform) currTransform.gameObject.SetActive(input);
    }
}
