using UnityEngine;
using UnityEngine.UI;
public class PlayerUI : MonoBehaviour
{
    private IDamageable owner;
    private Slider healthbar;
    private void Awake()
    {
        owner = GetComponentInParent<IDamageable>();
        if (owner == null)
        {
            Debug.LogWarning("This Healthbar is not associated with any Damageable GameObject! Disabling!");
            enabled = false;
            gameObject.SetActive(false);
        }

        healthbar = GetComponentInChildren<Slider>();
        if (healthbar == null)
        {
            Debug.LogWarning("There is no Healthbar Slider with this Damageable GameObject! Disabling!");
            enabled = false;
            gameObject.SetActive(false);
        }
    }
    private void OnEnable()
    {
        owner.onHealthChange += UpdateHealthbar;
    }
    private void OnDisable()
    {
        owner.onHealthChange -= UpdateHealthbar;
    }
    void Start()
    {
        UpdateHealthbar();
    }

    private void UpdateHealthbar()
    {
        healthbar.minValue = 0.0f;
        healthbar.maxValue = owner.maxHealth;
        healthbar.value = owner.currHealth;
    }
}
