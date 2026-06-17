using UnityEngine;

public class HealthTester : MonoBehaviour
{
    [SerializeField] private Health.Runtime.Health _health;

    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 40), "TakeDamage")) _health.TakeDamage(1f);
        if (GUI.Button(new Rect(10, 60, 150, 40), "Heal")) _health.Heal(1f);
    }
}