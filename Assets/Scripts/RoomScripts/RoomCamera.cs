using UnityEngine;

public class RoomCamera : RoomComponent
{
    private Camera mainCamera;
    private GameObject player;

    public bool isActive = true;
    public bool followPlayer = true;
    public bool stayInBounds = false;

    [SerializeField] private GameObject boundsLower;
    [SerializeField] private GameObject boundsUpper;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = Camera.main;
        player = GameObject.Find("Player");

        SanitizeBounds();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;
        Vector3 finalPos = mainCamera.transform.position;

        if (followPlayer)
        {
            finalPos = new Vector3(player.transform.position.x, player.transform.position.y, -10.0f);
        }
        if (stayInBounds)
        {
            finalPos.x = Mathf.Clamp(finalPos.x, boundsLower.transform.position.x, boundsUpper.transform.position.x);
            finalPos.y = Mathf.Clamp(finalPos.y, boundsLower.transform.position.y, boundsUpper.transform.position.y);
        }

        mainCamera.transform.position = finalPos;
    }

    private void SanitizeBounds()
    {
        if (boundsLower == null)
        {
            Debug.LogError("Camera Lower Bound is Null! Falling back to this room's Position");
            boundsLower = this.gameObject;
        }
        if (boundsUpper == null)
        {
            Debug.LogError("Camera Upper Bound is Null! Falling back to this room's Position");
            boundsUpper = this.gameObject;
        }
        
        float lowerX = boundsLower.transform.position.x;
        float lowerY = boundsLower.transform.position.y;
        float higherX = boundsUpper.transform.position.x;
        float higherY = boundsUpper.transform.position.y;

        boundsLower.transform.position = new Vector3(Mathf.Min(lowerX, higherX), Mathf.Min(lowerY, higherY), -10.0f);
        boundsUpper.transform.position = new Vector3(Mathf.Max(lowerX, higherX), Mathf.Max(lowerY, higherY), -10.0f);
    }
}