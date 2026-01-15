using System;
using UnityEngine;

public class CoinWallet : MonoBehaviour
{
    public int Coins { get; private set; }

    public event Action<int> OnCoinsChanged;

    public void AddCoins(int amount)
    {
        if (amount <= 0) return;
        Coins += amount;
        OnCoinsChanged?.Invoke(Coins);
    }

    public bool CanSpend(int amount) => amount > 0 && Coins >= amount;

    public bool SpendCoins(int amount)
    {
        if (!CanSpend(amount)) return false;
        Coins -= amount;
        OnCoinsChanged?.Invoke(Coins);
        return true;
    }
}
