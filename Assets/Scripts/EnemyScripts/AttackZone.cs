using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AttackZoneManager : MonoBehaviour
{
    static public AttackZoneManager Instance;
    private GameObject attackZonesContainer;
    [SerializeField, Range(0, 15)] private int attackZoneNumberLimit = 10;

    [SerializeField] private string attackZoneLayer;
    [SerializeField, ReadOnly] private List<BoxCollider2D> attackZones;
    [SerializeField, ReadOnly] private List<bool> attackZonesIsAvailable;
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
        attackZones = new List<BoxCollider2D>();
        attackZonesIsAvailable = new List<bool>();
    }
    private bool AddAttackZone()
    {
        if (attackZones.Count >= attackZoneNumberLimit)
        {
            Debug.LogWarning($"Already generated {attackZones.Count} Attack Zones, which reached/ exceeded the limit!");
            return false;
        }
        Debug.Log("Generating new Attack Zone");
        GameObject newAttackZone = new GameObject($"AttackZone_{attackZones.Count}");
        newAttackZone.layer = LayerMask.NameToLayer(attackZoneLayer);
        newAttackZone.transform.parent = attackZonesContainer.transform;
        newAttackZone.SetActive(false);

        BoxCollider2D newCollider = newAttackZone.AddComponent<BoxCollider2D>();
        newCollider.isTrigger = true;
        attackZones.Add(newCollider);

        attackZonesIsAvailable.Add(true);
        return true;
    }

    public void SetAttackZone(Vector3 position, Vector3 direction, float length, float height, float time)
    {
        int index = attackZonesIsAvailable.FindIndex(x => x);
        if (index <= -1)
        {
            if (AddAttackZone()) SetAttackZone(position, direction, length, height, time);
            return;
        }

        BoxCollider2D collider = attackZones[index];
        attackZonesIsAvailable[index] = false;

        collider.transform.position = position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        collider.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        collider.size = new Vector2(height, length);
        StartCoroutine(SetActive(index, time));
    }

    private IEnumerator SetActive(int index, float duration)
    {
        attackZones[index].gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        attackZones[index].gameObject.SetActive(false);
        attackZonesIsAvailable[index] = true;
    }
}
