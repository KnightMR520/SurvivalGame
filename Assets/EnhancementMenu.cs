using TMPro;
using UnityEngine;

public class EnhancementsMenu : MonoBehaviour
{
    public TextMeshProUGUI coinDisplay;
    public StatUI damageStatUI;
    public StatUI speedStatUI;
    public StatUI healthStatUI;
    public StatUI armorStatUI;
    public StatUI xpIncreaseStatUI;
    public StatUI attackSpeedStatUI;
    private int totalCoins;

    void Start()
    {
        //ResetPlayerData(); // Reset coins and levels at the start
        LoadPlayerData();  // Load saved player data when enhancements menu opens
        UpdateCoinDisplay();
        UpdateAllStatUI();
    }

    // Save the player's data when upgrading a stat
    private void SavePlayerData()
    {
        PlayerPrefs.SetInt("Coins", totalCoins);  // Save the player's total coins
        PlayerPrefs.SetInt("DamageLevel", damageStatUI.stat.GetLevel());
        PlayerPrefs.SetInt("SpeedLevel", speedStatUI.stat.GetLevel());
        PlayerPrefs.SetInt("HealthLevel", healthStatUI.stat.GetLevel());
        PlayerPrefs.SetInt("ArmorLevel", armorStatUI.stat.GetLevel());
        PlayerPrefs.SetInt("XPIncreaseLevel", xpIncreaseStatUI.stat.GetLevel());
        PlayerPrefs.SetInt("AttackSpeedLevel", attackSpeedStatUI.stat.GetLevel());

        PlayerPrefs.Save();  // Save changes immediately
    }

    // Load the player's saved data
    private void LoadPlayerData()
    {
        totalCoins = PlayerPrefs.GetInt("Coins", 999999);  // Default to 999999 coins if no data exists
        damageStatUI.stat.SetLevel(PlayerPrefs.GetInt("DamageLevel", 0)); // Default level 0
        speedStatUI.stat.SetLevel(PlayerPrefs.GetInt("SpeedLevel", 0));
        healthStatUI.stat.SetLevel(PlayerPrefs.GetInt("HealthLevel", 0));
        armorStatUI.stat.SetLevel(PlayerPrefs.GetInt("ArmorLevel", 0));
        xpIncreaseStatUI.stat.SetLevel(PlayerPrefs.GetInt("XPIncreaseLevel", 0));
        attackSpeedStatUI.stat.SetLevel(PlayerPrefs.GetInt("AttackSpeedLevel", 0));
    }

    // Method to reset player data
    public void ResetPlayerData()
    {
        // Set total coins to 999999
        totalCoins = 999999;
        PlayerPrefs.SetInt("Coins", totalCoins); // Save coins

        // Reset all levels to 0
        damageStatUI.stat.SetLevel(0);
        speedStatUI.stat.SetLevel(0);
        healthStatUI.stat.SetLevel(0);
        armorStatUI.stat.SetLevel(0);
        xpIncreaseStatUI.stat.SetLevel(0);
        attackSpeedStatUI.stat.SetLevel(0);

        // Save all reset data
        PlayerPrefs.SetInt("DamageLevel", 0);
        PlayerPrefs.SetInt("SpeedLevel", 0);
        PlayerPrefs.SetInt("HealthLevel", 0);
        PlayerPrefs.SetInt("ArmorLevel", 0);
        PlayerPrefs.SetInt("XPIncreaseLevel", 0);
        PlayerPrefs.SetInt("AttackSpeedLevel", 0);

        PlayerPrefs.Save();  // Save changes immediately

        // Update the UI
        UpdateCoinDisplay();
        UpdateAllStatUI();
    }

    // Updates the coin display text
    void UpdateCoinDisplay()
    {
        coinDisplay.text = "Coins: " + totalCoins;
        Debug.Log("Coins: " + totalCoins);
    }

    // Separate upgrade functions for each stat, triggered by clicking the stat icons
    public void UpgradeDamage()
    {
        UpgradeStat(damageStatUI.stat);
    }

    public void UpgradeSpeed()
    {
        UpgradeStat(speedStatUI.stat);
    }

    public void UpgradeHealth()
    {
        UpgradeStat(healthStatUI.stat);
    }

    public void UpgradeArmor()
    {
        UpgradeStat(armorStatUI.stat);
    }

    public void UpgradeXPIncrease()
    {
        UpgradeStat(xpIncreaseStatUI.stat);
    }

    public void UpgradeAttackSpeed()
    {
        UpgradeStat(attackSpeedStatUI.stat);
    }

    // General method for upgrading a stat
    private void UpgradeStat(Stat stat)
    {
        if (totalCoins >= stat.GetCurrentCost())
        {
            totalCoins -= stat.GetCurrentCost();
            stat.Upgrade();
            UpdateCoinDisplay();
            UpdateAllStatUI();
            SavePlayerData(); // Save progress after upgrading
        }
        else
        {
            Debug.Log("Not enough coins");
        }
    }

    // Updates the display for all stats
    void UpdateAllStatUI()
    {
        damageStatUI.UpdateUI();
        speedStatUI.UpdateUI();
        healthStatUI.UpdateUI();
        armorStatUI.UpdateUI();
        xpIncreaseStatUI.UpdateUI();
        attackSpeedStatUI.UpdateUI();
    }
}
