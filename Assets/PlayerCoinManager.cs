using System.Collections;
using System.Collections.Generic;
using TMPro;  // Needed for TextMeshPro
using UnityEngine;
using UnityEngine.UI;  // Needed for UI Image

public class PlayerCoinManager : MonoBehaviour
{
    // The player's current coin count
    private int currentCoins = 0;

    // Reference to the TextMeshProUGUI component for displaying the coin count
    public TextMeshProUGUI coinText;

    // Reference to the Image component for the coin sprite (optional, if you want to do more with it)
    public Image coinImage;

    void Start()
    {
        // Initialize the coin counter text
        UpdateCoinUI();
    }

    // Method to add coins
    public void AddCoins(int amount)
    {
        currentCoins += amount;
        UpdateCoinUI();
    }

    // Method to update the coin counter UI
    private void UpdateCoinUI()
    {
        // Update the text component to reflect the current coin count
        coinText.text = currentCoins.ToString();
    }

    // Call this when the player dies or the game ends to save the total coins
    public void SaveCoins()
    {
        // Get the total coins from PlayerPrefs (or default to 0)
        int totalCoins = PlayerPrefs.GetInt("Coins", 0);
        int totalCoinsGained = PlayerPrefs.GetInt("TotalCoinsGained", 0);
        // Add current session's coins to the total
        totalCoins += currentCoins;
        totalCoinsGained += currentCoins;
        // Save the new total
        PlayerPrefs.SetInt("Coins", totalCoins);
        PlayerPrefs.Save();
        PlayerPrefs.SetInt("TotalCoinsGained", totalCoinsGained);
        PlayerPrefs.Save();

        Debug.Log("Coins saved! Total Coins: " + totalCoins);
    }
}
