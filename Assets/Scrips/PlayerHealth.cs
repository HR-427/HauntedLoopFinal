using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int health;

    // Event the UI can listen to
    public event Action<int, int> OnHealthChanged;

    void Start()
    {
        health = maxHealth;
        OnHealthChanged?.Invoke(health, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        health = Mathf.Clamp(health - amount, 0, maxHealth);
        Debug.Log("Player health is now: " + health);
        OnHealthChanged?.Invoke(health, maxHealth);
        Debug.Log($"PlayerHealth instance {GetInstanceID()} health is now: {health}");

    }

    public void Heal(int amount)
    {
        health = Mathf.Clamp(health + amount, 0, maxHealth);
        OnHealthChanged?.Invoke(health, maxHealth);
    }

    void Die()
    {
        Debug.Log("Player died");
    }
}
