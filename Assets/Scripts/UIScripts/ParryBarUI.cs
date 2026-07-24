using UnityEngine;
using UnityEngine.UI;
public class ParryBarUI : MonoBehaviour
{
    private Enemy enemy;
    private Slider parryBar;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
        if (enemy == null)
        {
            Debug.LogWarning("This ParryBar is not associated with any Enemy GameObject! Disabling!");
            enabled = false;
            gameObject.SetActive(false);
        }

        parryBar = this.GetComponent<Slider>();
        if (parryBar == null)
        {
            Debug.LogWarning("There is no Slider Component! Disabling!");
            enabled = false;
            gameObject.SetActive(false);
        }
    }
    private void OnEnable()
    {
        enemy.enemyEvents.parryProgressChanges += UpdateParryBar;
    }
    private void OnDisable()
    {
        enemy.enemyEvents.parryProgressChanges += UpdateParryBar;
    }
    void Start()
    {
        UpdateParryBar();
    }

    private void UpdateParryBar()
    {
        parryBar.minValue = 0.0f;
        parryBar.maxValue = enemy.parry.parryTarget;
        parryBar.value = enemy.parry.parryProgress;
    }
}
