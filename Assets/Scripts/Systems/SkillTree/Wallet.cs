using UnityEngine;

public static class Wallet
{
    private const string CoinsKey = "WalletCoins";

    public static int Coins => PlayerPrefs.GetInt(CoinsKey, 0);
    public static int RunEarnings { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        RunEarnings = 0;
    }

    public static void Add(int amount)
    {
        if (amount <= 0) return;

        RunEarnings += amount;
        Set(Coins + amount);
    }

    public static bool TrySpend(int amount)
    {
        if (Coins < amount) return false;
        Set(Coins - amount);
        return true;
    }

    public static void ResetRun()
    {
        RunEarnings = 0;
    }

    public static void Clear()
    {
        Set(0);
    }

    static void Set(int value)
    {
        PlayerPrefs.SetInt(CoinsKey, value);
        PlayerPrefs.Save();
    }
}
