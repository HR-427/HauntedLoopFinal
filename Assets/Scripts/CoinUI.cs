using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinUI : MonoBehaviour
{
    [Header("Refs")]
    public CoinWallet wallet;
    public Image fillImage;          
    public TMP_Text coinCountText;   

    [Header("Bar Settings")]
    public int barMaxCoins = 100;

    void OnEnable()
    {
        if (wallet != null)
            wallet.OnCoinsChanged += Refresh;

        if (wallet != null)
            Refresh(wallet.Coins);
    }

    void OnDisable()
    {
        if (wallet != null)
            wallet.OnCoinsChanged -= Refresh;
    }

    void Refresh(int coins)
    {
        if (coinCountText != null)
            coinCountText.text = "coins: " + coins;

        if (fillImage != null)
        {
            float fill = Mathf.Clamp01(coins / (float)barMaxCoins);
            fillImage.fillAmount = fill;
        }
    }

}
