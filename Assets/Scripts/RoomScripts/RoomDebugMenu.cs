using UnityEngine;
using UnityEngine.InputSystem;
public class RoomDebugMenu : RoomComponent
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
        room.roomEvents.roomStarts?.Invoke();
    }
    public void EndRoom()
    {
        room.roomEvents.roomEnds?.Invoke();
    }
    public void KillAllEnemies()
    {
        room.waveSpawner.KillCurrentEnemies();
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
