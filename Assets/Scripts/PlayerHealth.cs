using UnityEngine;
using System;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int health;
    public bool takeDamage = true;

    [Header("Shop (Optional)")]
    public GameObject healthShopPanel;
    public bool pauseOnShop = true;

    [Header("Default Popup (Optional)")]
    public GameObject damagePopupPrefab; 
    public float popupHeight = 1.6f;

    public event Action<int, int> OnHealthChanged;
    public event Action OnDied;

    bool isDeadOrShopping;

    void Start()
    {
        health = maxHealth;
        isDeadOrShopping = false;

        if (healthShopPanel != null)
            healthShopPanel.SetActive(false);

        OnHealthChanged?.Invoke(health, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        TakeDamage(amount, null);
    }

    public void TakeDamage(int amount, GameObject popupPrefabOverride)
    {
        if (!takeDamage || isDeadOrShopping || amount <= 0) return;

        health = Mathf.Clamp(health - amount, 0, maxHealth);

        SpawnPopup(amount, popupPrefabOverride);

        OnHealthChanged?.Invoke(health, maxHealth);

        if (health <= 0)
            Die();
    }

    public bool SpendHealth(int amount)
    {
        return SpendHealth(amount, null);
    }

    public bool SpendHealth(int amount, GameObject popupPrefabOverride)
    {
        if (isDeadOrShopping || amount <= 0) return false;

        if (health - amount <= 0)
        {
            health = 0;
            OnHealthChanged?.Invoke(health, maxHealth);
            Die();
            return false;
        }

        health = Mathf.Clamp(health - amount, 0, maxHealth);

        SpawnPopup(amount, popupPrefabOverride);

        OnHealthChanged?.Invoke(health, maxHealth);
        return true;
    }

    public void Heal(int amount)
    {
        if (amount <= 0) return;

        health = Mathf.Clamp(health + amount, 0, maxHealth);
        OnHealthChanged?.Invoke(health, maxHealth);
    }

    public void ReviveTo(int targetHealth)
    {
        targetHealth = Mathf.Clamp(targetHealth, 1, maxHealth);

        health = targetHealth;
        OnHealthChanged?.Invoke(health, maxHealth);

        ResumeFromShop();
    }


    void Die()
    {
        if (isDeadOrShopping) return;

        isDeadOrShopping = true;
        takeDamage = false;

        OnDied?.Invoke();

        if (healthShopPanel != null)
        {
            healthShopPanel.SetActive(true);
            if (pauseOnShop) Time.timeScale = 0f;
        }
    }

    public void ResumeFromShop()
    {
        isDeadOrShopping = false;
        takeDamage = true;

        if (healthShopPanel != null)
            healthShopPanel.SetActive(false);

        if (pauseOnShop)
            Time.timeScale = 1f;
    }

    void SpawnPopup(int amount, GameObject popupOverride)
    {
        GameObject prefabToUse = popupOverride != null ? popupOverride : damagePopupPrefab;
        if (prefabToUse == null) return;

        Vector3 worldPos = transform.position + Vector3.up * popupHeight;

        GameObject popup = Instantiate(prefabToUse);

        RectTransform rt = popup.GetComponent<RectTransform>();
        if (rt != null)
        {
            Canvas canvas = FindAnyObjectByType<Canvas>();
            if (canvas == null)
            {
                Destroy(popup);
                return;
            }

            popup.transform.SetParent(canvas.transform, false);

            Camera cam = Camera.main;
            rt.position = cam != null
                ? cam.WorldToScreenPoint(worldPos)
                : new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        }
        else
        {
            popup.transform.position = worldPos;
            if (Camera.main != null)
                popup.transform.rotation = Camera.main.transform.rotation;
        }

        TMP_Text tmp = popup.GetComponentInChildren<TMP_Text>();
        if (tmp != null)
            tmp.text = $"-{amount}";
    }
}
