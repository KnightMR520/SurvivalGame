using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class StatUI
{
    public Stat stat;           // Holds the stat data (level, cost, etc.)
    public TextMeshProUGUI statName;       // Displays the stat name (e.g., "Damage")
    public TextMeshProUGUI costText;       // Displays the current cost of the next level
    public Image[] levelBoxes;  // Array of images for the 10 level boxes
    public TextMeshProUGUI nextLevelText;  // Displays the next level description
    public Button upgradeButton; // Button to trigger the upgrade

    public void UpdateUI()
    {
        // statName.text = stat.statName;
        costText.text = "Cost: " + stat.GetCurrentCost();
        nextLevelText.text = stat.GetNextLevelDescription();

        // Update the level boxes
        for (int i = 0; i < levelBoxes.Length; i++)
        {
            levelBoxes[i].color = (i < stat.GetLevel()) ? Color.green : Color.gray;
        }

        // Disable the upgrade button if max level is reached
        upgradeButton.interactable = !stat.IsMaxLevel();
    }
}

[System.Serializable]
public class Stat
{
    public string statName;  // Name of the stat (e.g., "Damage")
    public int level = 0;    // Current level of the stat
    public int maxLevel = 10;  // Maximum level the stat can reach
    public int baseCost = 100; // Base cost of the stat at level 1

    // Returns the cost of upgrading the stat at the current level
    public int GetCurrentCost()
    {
        return baseCost + (level * 100);  // Example cost scaling
    }

    // Upgrades the stat by 1 level
    public void Upgrade()
    {
        if (level < maxLevel)
        {
            level++;
        }
    }

    // Returns true if the stat is at max level
    public bool IsMaxLevel()
    {
        return level >= maxLevel;
    }

    // Returns the description of the next level
    public string GetNextLevelDescription()
    {
        if (level < maxLevel)
        {
            return "Next Level: " + (level + 1);
        }
        else
        {
            return "Max level reached";
        }
    }

    // Returns the current level of the stat
    public int GetLevel()
    {
        return level;
    }

    // New SetLevel function to set the stat's level
    public void SetLevel(int levelGiven)
    {
        level = levelGiven;
    }
}
