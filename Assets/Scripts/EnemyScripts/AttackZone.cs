using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
public class AttackZoneManager : MonoBehaviour
{
    static public AttackZoneManager Instance;
    private GameObject attackZonesContainer;
    [SerializeField, Range(0, 6)] private int initialZoneNumber = 3;
    [SerializeField, Range(0, 15)] private int attackZoneNumberLimit = 10;

    [SerializeField] private string attackZoneLayer;
    [SerializeField, ReadOnly] private List<BoxCollider2D> squareAttackZones;
    [SerializeField, ReadOnly] private List<bool> squareAttackZonesIsAvailable;

    [SerializeField, ReadOnly] private List<CircleCollider2D> circleAttackZones;
    [SerializeField, ReadOnly] private List<bool> circleAttackZonesIsAvailable;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("A Second AttackZoneManager tried to initialize! Deleting it!");
            return;
        }
        else
        {
            AttackZoneManager.Instance = this;
            Initialize();
        }
    }

    private void Initialize()
    {
        attackZonesContainer = new GameObject("AttackZoneContainer");
        attackZonesContainer.transform.parent = this.transform;
        attackZonesContainer.isStatic = true;
        squareAttackZones = new List<BoxCollider2D>();
        squareAttackZonesIsAvailable = new List<bool>();

        circleAttackZones = new List<CircleCollider2D>();
        circleAttackZonesIsAvailable = new List<bool>();
        for (int i = 0; i < initialZoneNumber; i++) AddSquareAttackZone();
        for (int i = 0; i < initialZoneNumber; i++) AddCircleAttackZone();
    }
    private bool AddSquareAttackZone()
    {
        if (squareAttackZones.Count >= attackZoneNumberLimit)
        {
            Debug.LogWarning($"Already generated {squareAttackZones.Count} Attack Zones, which reached/ exceeded the limit!");
            return false;
        }
        Debug.Log("Generating new Square Attack Zone");
        GameObject newAttackZone = new GameObject($"SquareAttackZone_{squareAttackZones.Count}");
        newAttackZone.layer = LayerMask.NameToLayer(attackZoneLayer);
        newAttackZone.transform.parent = attackZonesContainer.transform;
        newAttackZone.isStatic = true;

        BoxCollider2D newCollider = newAttackZone.AddComponent<BoxCollider2D>();
        newCollider.isTrigger = true;
        newCollider.enabled = false;
        squareAttackZones.Add(newCollider);

        squareAttackZonesIsAvailable.Add(true);
        return true;
    }
    private bool AddCircleAttackZone()
    {
        if (circleAttackZones.Count >= attackZoneNumberLimit)
        {
            Debug.LogWarning($"Already generated {circleAttackZones.Count} Attack Zones, which reached/ exceeded the limit!");
            return false;
        }
        Debug.Log("Generating new Circle Attack Zone");
        GameObject newAttackZone = new GameObject($"CircleAttackZone_{circleAttackZones.Count}");
        newAttackZone.layer = LayerMask.NameToLayer(attackZoneLayer);
        newAttackZone.transform.parent = attackZonesContainer.transform;
        newAttackZone.isStatic = true;

        CircleCollider2D newCollider = newAttackZone.AddComponent<CircleCollider2D>();
        newCollider.isTrigger = true;
        newCollider.enabled = false;
        circleAttackZones.Add(newCollider);

        circleAttackZonesIsAvailable.Add(true);
        return true;
    }

    public void SetSquareAttackZone(Vector3 position, Vector3 direction, float length, float height, float time)
    {
        int index = squareAttackZonesIsAvailable.FindIndex(x => x);
        if (index <= -1)
        {
            if (AddSquareAttackZone()) SetSquareAttackZone(position, direction, length, height, time);
            return;
        }

        BoxCollider2D collider = squareAttackZones[index];
        squareAttackZonesIsAvailable[index] = false;

        collider.transform.position = position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        collider.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        collider.size = new Vector2(height, length);
        StartCoroutine(SquareSetActive(index, time));
    }

    public void SetCircleAttackZone(Vector3 position, float radius, float time)
    {
        int index = circleAttackZonesIsAvailable.FindIndex(x => x);
        if (index <= -1)
        {
            if (AddCircleAttackZone()) SetCircleAttackZone(position, radius, time);
            return;
        }

        CircleCollider2D collider = circleAttackZones[index];
        circleAttackZonesIsAvailable[index] = false;

        collider.transform.position = position;
        collider.radius = radius;
        StartCoroutine(CircleSetActive(index, time));
    }

    private IEnumerator SquareSetActive(int index, float duration)
    {
        squareAttackZones[index].enabled = true;
        yield return new WaitForSeconds(duration);
        squareAttackZones[index].enabled = false;
        squareAttackZonesIsAvailable[index] = true;
    }
    private IEnumerator CircleSetActive(int index, float duration)
    {
        circleAttackZones[index].enabled = true;
        yield return new WaitForSeconds(duration);
        circleAttackZones[index].enabled = false;
        circleAttackZonesIsAvailable[index] = true;
    }
}
