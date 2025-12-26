using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [Header("Variables")]
    public int maxHealth = 100;
    public int health;
    public bool takeDamage;

    [Header("UI")]
    public GameObject depletedText;

    [Header("References")]
    public SpellCaster spellCast;

    public event Action<int, int> OnHealthChanged;

    void Start()
    {
        health = maxHealth;
        takeDamage = true;

        if (depletedText != null)
            depletedText.SetActive(false);

        OnHealthChanged?.Invoke(health, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (!takeDamage) return;

        spellCast.isCasting = true;

        health = Mathf.Clamp(health - amount, 0, maxHealth);
        Debug.Log("Player health is now: " + health);
        OnHealthChanged?.Invoke(health, maxHealth);

        if (health == 0)
        {
            Die();
        }
    }

    void Die()
    {
        takeDamage = false;
        spellCast.isCasting = false;

        if (depletedText != null)
            depletedText.SetActive(true);

        Debug.Log("Player died");
    }
}
