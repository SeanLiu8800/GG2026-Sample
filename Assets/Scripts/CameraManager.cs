using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    [SerializeField] private Camera Camera;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Another CameraManager tried to Initialize! Deleting it!");
            return;
        }
        else
        {
            Instance = this;
        }
    }


}
