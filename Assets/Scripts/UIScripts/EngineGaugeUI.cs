using UnityEngine;
using UnityEngine.UI;
public class EngineGaugeUI : MonoBehaviour
{
    private Player player;
    private Slider engineGauge;
    private void Awake()
    {
        engineGauge = this.GetComponent<Slider>();
        if (engineGauge == null)
        {
            Debug.LogWarning("There is no EngineGauge Slider with this Damageable GameObject! Disabling!");
            enabled = false;
            gameObject.SetActive(false);
        }

        player = GetComponentInParent<Player>();
        if (player == null)
        {
            Debug.LogWarning("There is no Player script component with this Damageable GameObject! Disabling!");
            enabled = false;
            gameObject.SetActive(false);
        }
    }
    private void OnEnable()
    {
        player.playerEvents.engineValueChanges += UpdateEngineGauge;
    }
    private void OnDisable()
    {
        player.playerEvents.engineValueChanges -= UpdateEngineGauge;
    }
    void Start()
    {
        UpdateEngineGauge();
    }

    private void UpdateEngineGauge()
    {
        engineGauge.minValue = 0.0f;
        engineGauge.maxValue = player.attack.maxEngine;
        engineGauge.value = player.attack.currEngine;
    }
}
