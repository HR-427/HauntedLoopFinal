using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public Image healthFill;

    void Start()
    {
        if (playerHealth == null)
        {
            Debug.LogError("PlayerHealth NOT assigned to health bar");
            return;
        }

        playerHealth.OnHealthChanged += UpdateHealthBar;
        Debug.Log("Health bar subscribed");
    }

    void OnDestroy()
    {
        playerHealth.OnHealthChanged -= UpdateHealthBar;
    }

    void UpdateHealthBar(int current, int max)
    {
        Debug.Log($"Health bar update: {current}/{max}");
        healthFill.fillAmount = (float)current / max;
        Debug.Log($"Health bar listening to instance {playerHealth.GetInstanceID()} → {current}/{max}");

    }
}
