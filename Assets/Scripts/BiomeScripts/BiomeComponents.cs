using UnityEngine;

[CreateAssetMenu(fileName = "BiomeComponents", menuName = "Scriptable Objects/BiomeComponents")]
public class BiomeComponents : ScriptableObject
{
    public GameObject[] biomeCombatRooms;
    public GameObject[] biomeEventRooms;

    [Range(1, 10)] public int biomeLength = 5;
}
