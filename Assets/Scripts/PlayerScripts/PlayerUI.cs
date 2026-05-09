using UnityEngine;
using UnityEngine.UI;
public class PlayerUI : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Slider healthbar;
    private void OnEnable()
    {
        player.health.onHealthChange += UpdateHealthbar;
    }
    private void OnDisable()
    {
        player.health.onHealthChange -= UpdateHealthbar;
    }
    void Start()
    {
        UpdateHealthbar();
    }

    private void UpdateHealthbar()
    {
        healthbar.value = player.health.currHealth;
        healthbar.minValue = 0.0f;
        healthbar.maxValue = player.health.maxHealth;
    }
}
