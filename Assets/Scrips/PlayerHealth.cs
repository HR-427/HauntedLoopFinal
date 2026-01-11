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

    [Header("Floating Sprite Popup")]
    public GameObject floatingTextPrefab;   // SpriteRenderer popup prefab
    public Transform textSpawnPoint;         // optional: empty above player head

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
        Debug.Log($"[PlayerHealth] TakeDamage called amount={amount} takeDamage={takeDamage} prefab={(floatingTextPrefab ? floatingTextPrefab.name : "NULL")}");

        if (!takeDamage)
        {
            Debug.LogWarning("[PlayerHealth] takeDamage is FALSE so damage + popup are blocked.");
            return;
        }

        health -= amount;
        health = Mathf.Clamp(health, 0, maxHealth);

        OnHealthChanged?.Invoke(health, maxHealth);

        SpawnFloatingText(amount);

        if (health <= 0)
            Die();
    }

    void Die()
    {
        takeDamage = false;

        if (spellCast != null)
            spellCast.isCasting = false;

        if (depletedText != null)
            depletedText.SetActive(true);

        Debug.Log("Player died");
    }

    void SpawnFloatingText(int amount)
    {
        if (floatingTextPrefab == null)
        {
            Debug.LogWarning("[PlayerHealth] floatingTextPrefab is NULL (assign the prefab in Inspector).");
            return;
        }

        Vector3 spawnPos = textSpawnPoint != null
            ? textSpawnPoint.position
            : transform.position + Vector3.up * 1.8f;

        // Spawn with any rotation (we'll override it next)
        GameObject popupObj = Instantiate(
            floatingTextPrefab,
            spawnPos,
            Quaternion.identity
        );

        // ✅ PUT THIS CODE RIGHT HERE
        if (Camera.main != null)
        {
            popupObj.transform.rotation =
                Camera.main.transform.rotation * Quaternion.Euler(0f, 180f, 0f);
        }

        Debug.Log($"[PlayerHealth] Spawned: {popupObj.name} active={popupObj.activeInHierarchy}");
    }

}
