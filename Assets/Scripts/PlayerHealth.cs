using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [Header("Variables")]
    public int maxHealth = 100;
    public int health;
    public bool takeDamage = true;

    [Header("UI")]
    public GameObject depletedText;

    [Header("References")]
    public SpellCaster spellCast;

    [Header("Damage Floating Text (for enemy hits)")]
    public GameObject damageFloatingTextPrefab; // this is your "-10" prefab
    public Transform textSpawnPoint;
    public Vector3 textOffset = new Vector3(0f, 1.6f, 0f);

    public event Action<int, int> OnHealthChanged;

    void Start()
    {
        health = maxHealth;
        takeDamage = true;

        if (depletedText != null)
            depletedText.SetActive(false);

        OnHealthChanged?.Invoke(health, maxHealth);
    }

    void Update()
    {
        if (health <= 0)
        {
            health = 0;
            takeDamage = false;

            if (spellCast != null)
                spellCast.isCasting = false;

            if (depletedText != null)
                depletedText.SetActive(true);
        }
    }

    // ✅ Use this for ENEMY damage (shows damage text)
    public void TakeDamage(int amount)
    {
        if (!takeDamage) return;
        if (amount <= 0) return;

        health -= amount;
        health = Mathf.Clamp(health, 0, maxHealth);

        ShowDamageText(amount);
        OnHealthChanged?.Invoke(health, maxHealth);
    }

    // ✅ Use this for SPELL COSTS (NO damage text)
    public void SpendHealth(int amount)
    {
        if (!takeDamage) return;
        if (amount <= 0) return;

        health -= amount;
        health = Mathf.Clamp(health, 0, maxHealth);

        OnHealthChanged?.Invoke(health, maxHealth);
    }

    void ShowDamageText(int amount)
    {
        if (damageFloatingTextPrefab == null) return;

        Transform spawnT = textSpawnPoint != null ? textSpawnPoint : transform;
        Vector3 pos = spawnT.position + textOffset;

        GameObject popupObj = Instantiate(damageFloatingTextPrefab, pos, Quaternion.identity);

        // Face the camera (flip 180 if your prefab is backwards)
        if (Camera.main != null)
        {
            popupObj.transform.rotation =
                Camera.main.transform.rotation * Quaternion.Euler(0f, 180f, 0f);
        }

        // Optional: if your prefab has TMP/TextMesh, this will update it to "-amount"
        // If your prefab already has "-10" baked in, this won't hurt (you can delete this part if you want).
        var tmp = popupObj.GetComponentInChildren<TMPro.TextMeshPro>();
        if (tmp != null) tmp.text = "-" + amount.ToString();

        var tm = popupObj.GetComponentInChildren<TextMesh>();
        if (tm != null) tm.text = "-" + amount.ToString();
    }
}
