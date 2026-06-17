using UnityEngine;
using UnityEngine.UI;
using HealthSystem = Health.Runtime.Health;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image _fill;
    [SerializeField] private HealthSystem _health;

    private void OnEnable()
    {
        _health.OnHPChangeNormalized += UpdateBar;
    }

    private void OnDisable()
    {
        _health.OnHPChangeNormalized -= UpdateBar;
    }

    private void UpdateBar(float normalized)
    {
        _fill.fillAmount = normalized;
    }
}