using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerHealth;
    public Image healthFill;

    void OnEnable()
    {

        playerHealth.OnHealthChanged += UpdateHealthBar;

        UpdateHealthBar(playerHealth.health, playerHealth.maxHealth);
    }

    void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.OnHealthChanged -= UpdateHealthBar;
    }

    void UpdateHealthBar(int current, int max)
    {
        healthFill.fillAmount = Mathf.Clamp01((float)current / max);
    }
}
