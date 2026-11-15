using UnityEngine;
using TMPro;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private HealthSO healthSO;
    private TextMeshProUGUI healthText;

    private void Awake()
    {
        healthText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (healthSO != null && healthText != null)
        {
            healthText.text = $"Health: {healthSO.CurrentHealth}";
        }
    }
}