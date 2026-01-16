using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class HealthShopManager : MonoBehaviour
{
    [Header("Refs")]
    public PlayerHealth playerHealth;
    public CoinWallet wallet;

    [Header("UI")]
    public GameObject shopPanel;
    public TMP_Text coinText;
    public TMP_Text messageText;
    public GameObject shopContentRoot;

    [Header("No Coins Behaviour")]
    public float noCoinsShowSeconds = 2f;

    bool isHandlingDeath;

    void Awake()
    {
        if (playerHealth == null)
            playerHealth = FindAnyObjectByType<PlayerHealth>();

        if (wallet == null)
            wallet = FindAnyObjectByType<CoinWallet>();
    }

    void OnEnable()
    {
        if (playerHealth != null)
            playerHealth.OnDied += OnPlayerDied;

        if (wallet != null)
            wallet.OnCoinsChanged += OnCoinsChanged;
    }

    void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.OnDied -= OnPlayerDied;

        if (wallet != null)
            wallet.OnCoinsChanged -= OnCoinsChanged;
    }

    void OnCoinsChanged(int coins)
    {
        if (coinText != null)
            coinText.text = coins.ToString();
    }

    void OnPlayerDied()
    {
        if (isHandlingDeath) return;
        isHandlingDeath = true;

        if (shopPanel != null) shopPanel.SetActive(true);
        Time.timeScale = 0f;

        if (wallet != null && wallet.Coins <= 0)
        {
            if (shopContentRoot != null) shopContentRoot.SetActive(false);
            if (coinText != null) coinText.gameObject.SetActive(false);

            if (messageText != null)
                messageText.text = "Out of health!\nNo coins available.";

            StartCoroutine(ReloadSceneAfterDelay());
            return;
        }

        if (shopContentRoot != null) shopContentRoot.SetActive(true);
        if (coinText != null) coinText.gameObject.SetActive(true);
        if (messageText != null) messageText.text = "";
        isHandlingDeath = false; 
    }

    IEnumerator ReloadSceneAfterDelay()
    {
        yield return new WaitForSecondsRealtime(noCoinsShowSeconds);

        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BuyHealth(int healthAmount)
    {
        if (wallet == null || playerHealth == null) return;

        int cost = GetCostForHealth(healthAmount);
        if (!wallet.SpendCoins(cost))
        {
            if (messageText != null) messageText.text = "Not enough coins!";
            return;
        }

        if (messageText != null) messageText.text = "";
        playerHealth.ReviveTo(healthAmount);

        if (shopPanel != null) shopPanel.SetActive(false);
        Time.timeScale = 1f;

        isHandlingDeath = false;
    }

    int GetCostForHealth(int healthAmount)
    {
        switch (healthAmount)
        {
            case 25: return 40;
            case 50: return 75;
            case 100: return 130;
            default: return 9999;
        }
    }
}
