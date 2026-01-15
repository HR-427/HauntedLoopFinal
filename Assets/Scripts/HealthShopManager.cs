using UnityEngine;
using TMPro;
using System.Collections;
using Unity.Cinemachine;

public class HealthShopManager : MonoBehaviour
{
    [Header("Refs")]
    public PlayerHealth playerHealth;
    public CoinWallet wallet;

    [Header("UI")]
    public GameObject shopPanel;
    public TMP_Text coinText;
    public TMP_Text messageText; 

    [Header("No Coins Behaviour")]
    public Transform startSpawnPoint;        
    public float noCoinsShowSeconds = 2f;    
    public int reviveHealthWhenNoCoins = 25;

    void Awake()
    {
        if (playerHealth == null)
            playerHealth = FindAnyObjectByType<PlayerHealth>();

        if (wallet == null && playerHealth != null)
            wallet = playerHealth.GetComponentInParent<CoinWallet>();

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
        if (shopPanel != null && shopPanel.activeInHierarchy)
            RefreshShopUI();
    }

    void OnPlayerDied()
    {
        if (wallet == null || playerHealth == null) return;

        if (shopPanel != null) shopPanel.SetActive(true);

        if (messageText != null) messageText.text = "";
        Time.timeScale = 0f;

        if (wallet.Coins <= 0)
        {
            if (messageText != null) messageText.text = "No coins...";
            StartCoroutine(NoCoinsRespawnRoutine());
            return;
        }

        RefreshShopUI();
    }

    IEnumerator NoCoinsRespawnRoutine()
    {
        yield return new WaitForSecondsRealtime(noCoinsShowSeconds);

        if (shopPanel != null) shopPanel.SetActive(false);

        RespawnAtStart_SnapCinemachine();

        Time.timeScale = 1f;
    }

    void RespawnAtStart_SnapCinemachine()
    {
        if (startSpawnPoint == null)
        {
            Debug.LogError("[HealthShopManager] startSpawnPoint not assigned.");
            return;
        }

        Transform playerRoot = playerHealth.transform.root;

        Vector3 oldPos = playerRoot.position;
        Vector3 newPos = startSpawnPoint.position;
        Vector3 delta = newPos - oldPos;

        playerRoot.position = newPos;
        playerRoot.rotation = startSpawnPoint.rotation;

        var brain = Camera.main != null ? Camera.main.GetComponent<Unity.Cinemachine.CinemachineBrain>() : null;
        if (brain != null)
            brain.ManualUpdate();

        playerHealth.ReviveTo(reviveHealthWhenNoCoins);
    }


    public void BuyHealth(int healthAmount)
    {
        if (wallet == null || playerHealth == null) return;

        int cost = GetCostForHealth(healthAmount);

        if (!wallet.SpendCoins(cost))
        {
            if (messageText != null) messageText.text = "Not enough coins!";
            RefreshShopUI();
            return;
        }

        if (messageText != null) messageText.text = "";

        playerHealth.ReviveTo(healthAmount);
        RefreshShopUI();
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

    void RefreshShopUI()
    {
        if (coinText != null && wallet != null)
            coinText.text = wallet.Coins.ToString();
    }
}
