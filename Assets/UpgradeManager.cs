using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class UpgradeOption
{
    public string upgradeName;
    public Sprite upgradeImage;
    public System.Action upgradeAction;
    public int currentLevel;
}

public class UpgradeManager : MonoBehaviour
{
    public GameObject upgradePanel;
    public Button upgradeButton1;
    public Button upgradeButton2;
    public Button upgradeButton3;
    public Image upgradeImage1;
    public Image upgradeImage2;
    public Image upgradeImage3;
    private PlayerShooting playerShooting;
    private BubbleShield bubbleShield;
    private Spark spark;
    private Ricochet ricochet;
    private player_script playerScript;
    private List<UpgradeOption> availableUpgrades = new List<UpgradeOption>();
    private List<UpgradeOption> selectedUpgrades = new List<UpgradeOption>();

    private void Start()
    {
        upgradePanel.SetActive(false);
        playerShooting = FindObjectOfType<PlayerShooting>();
        bubbleShield = FindObjectOfType<BubbleShield>();
        spark = FindObjectOfType<Spark>();
        ricochet = FindObjectOfType<Ricochet>();
        playerScript = FindObjectOfType<player_script>();

        // Add upgrades and load images from Resources folder
        availableUpgrades.Add(new UpgradeOption
        {
            upgradeName = "Volley",
            upgradeImage = Resources.Load<Sprite>("UpgradeIcons/VolleySprite"),
            upgradeAction = ApplyVolleyUpgrade,
            currentLevel = playerShooting.volleyLevel
        });
        availableUpgrades.Add(new UpgradeOption
        {
            upgradeName = "Bubble Shield",
            upgradeImage = Resources.Load<Sprite>("UpgradeIcons/BubbleShieldSprite"),
            upgradeAction = ApplyBubbleShieldUpgrade,
            currentLevel = bubbleShield.currentAuraLevel
        });
        availableUpgrades.Add(new UpgradeOption
        {
            upgradeName = "Spark",
            upgradeImage = Resources.Load<Sprite>("UpgradeIcons/SparkSprite"),
            upgradeAction = ApplySparkUpgrade,
            currentLevel = spark.sparkLevel
        });
        availableUpgrades.Add(new UpgradeOption
        {
            upgradeName = "Ricochet",
            upgradeImage = Resources.Load<Sprite>("UpgradeIcons/RicochetSprite"),
            upgradeAction = ApplyRicochetUpgrade,
            currentLevel = ricochet.currentRicochetLevel
        });
        availableUpgrades.Add(new UpgradeOption
        {
            upgradeName = "Move Speed",
            upgradeImage = Resources.Load<Sprite>("UpgradeIcons/SpeedSprite"),
            upgradeAction = ApplyMoveSpeedUpgrade,
            currentLevel = playerScript.currentMoveSpeedLevel
        });
        availableUpgrades.Add(new UpgradeOption
        {
            upgradeName = "Armor",
            upgradeImage = Resources.Load<Sprite>("UpgradeIcons/ArmorSprite"),
            upgradeAction = ApplyArmorUpgrade,
            currentLevel = playerScript.currentArmorLevel
        });
        availableUpgrades.Add(new UpgradeOption
        {
            upgradeName = "Global Damage",
            upgradeImage = Resources.Load<Sprite>("UpgradeIcons/GlobalDamageSprite"),
            upgradeAction = ApplyGlobalDamageUpgrade,
            currentLevel = playerScript.globalDamageLevel
        });

        Debug.Log($"Volley Level: {playerShooting.volleyLevel}");
        Debug.Log($"Bubble Shield Level: {bubbleShield.currentAuraLevel}");
        Debug.Log($"Spark Level: {spark.sparkLevel}");
        Debug.Log($"Ricochet Level: {ricochet.currentRicochetLevel}");
    }

    public void ShowUpgradePanel()
    {
        if (AreAllUpgradesMaxLevel()) return;

        Time.timeScale = 0f;
        upgradePanel.SetActive(true);

        selectedUpgrades = GetRandomUpgrades();
        Debug.Log("Upgrades fetched in show upgrade panel");
        Debug.Log($"Selected Upgrades Count: {selectedUpgrades.Count}");
        for (int i = 0; i < selectedUpgrades.Count; i++)
        {
            Debug.Log($"Upgrade {i}: {selectedUpgrades[i].upgradeName}, Level: {selectedUpgrades[i].currentLevel}");
        }
        UpdateUpgradeUI(selectedUpgrades);
    }

    private void HideUpgradePanel()
    {
        upgradePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void UpdateUpgradeUI(List<UpgradeOption> selectedUpgrades)
    {
        if (selectedUpgrades == null || selectedUpgrades.Count == 0)
        {
            Debug.LogError("Selected upgrades list is null or empty.");
            return;
        }

        // Helper function to set button visibility and reset text
        void SetButtonVisibility(Button button, Image image, TMP_Text text, bool isVisible)
        {
            Color buttonColor = button.image.color; // Get current button color

            if (isVisible)
            {
                button.interactable = true; // Make the button interactable
                buttonColor.a = 1; // Fully opaque
                if (text != null)
                {
                    text.color = Color.white; // Set text color to white (or any desired color)
                }
            }
            else
            {
                button.interactable = false; // Make the button non-interactable
                buttonColor.a = 0; // Fully transparent
                if (text != null)
                {
                    text.color = new Color(1, 1, 1, 0); // Set text color to transparent
                    text.text = ""; // Reset text to empty if not visible
                }
            }

            button.image.color = buttonColor; // Update button image color
        }

        // Set the images and text based on the available selected upgrades
        TMP_Text text1 = upgradeButton1.GetComponentInChildren<TMP_Text>(); // Get the TextMeshPro component
        if (selectedUpgrades.Count > 0)
        {
            upgradeImage1.sprite = selectedUpgrades[0].upgradeImage;
            text1.text = $"{selectedUpgrades[0].upgradeName} (Lv {selectedUpgrades[0].currentLevel + 1})";
            upgradeButton1.onClick.RemoveAllListeners(); // Clear previous listeners
            upgradeButton1.onClick.AddListener(() => { ApplySelectedUpgrade(0, selectedUpgrades); });
            SetButtonVisibility(upgradeButton1, upgradeImage1, text1, true);
        }
        else
        {
            SetButtonVisibility(upgradeButton1, upgradeImage1, text1, false);
        }

        TMP_Text text2 = upgradeButton2.GetComponentInChildren<TMP_Text>(); // Get the TextMeshPro component
        if (selectedUpgrades.Count > 1)
        {
            upgradeImage2.sprite = selectedUpgrades[1].upgradeImage;
            text2.text = $"{selectedUpgrades[1].upgradeName} (Lv {selectedUpgrades[1].currentLevel + 1})";
            upgradeButton2.onClick.RemoveAllListeners(); // Clear previous listeners
            upgradeButton2.onClick.AddListener(() => { ApplySelectedUpgrade(1, selectedUpgrades); });
            SetButtonVisibility(upgradeButton2, upgradeImage2, text2, true);
        }
        else
        {
            SetButtonVisibility(upgradeButton2, upgradeImage2, text2, false);
        }

        TMP_Text text3 = upgradeButton3.GetComponentInChildren<TMP_Text>(); // Get the TextMeshPro component
        if (selectedUpgrades.Count > 2)
        {
            upgradeImage3.sprite = selectedUpgrades[2].upgradeImage;
            text3.text = $"{selectedUpgrades[2].upgradeName} (Lv {selectedUpgrades[2].currentLevel + 1})";
            upgradeButton3.onClick.RemoveAllListeners(); // Clear previous listeners
            upgradeButton3.onClick.AddListener(() => { ApplySelectedUpgrade(2, selectedUpgrades); });
            SetButtonVisibility(upgradeButton3, upgradeImage3, text3, true);
        }
        else
        {
            SetButtonVisibility(upgradeButton3, upgradeImage3, text3, false);
        }
    }

    private List<UpgradeOption> GetRandomUpgrades()
    {
        List<UpgradeOption> availableOptions = availableUpgrades.FindAll(option => option.currentLevel < 5);

        Debug.Log($"Available Upgrades Count: {availableOptions.Count}");
        List<UpgradeOption> selectedOptions = new List<UpgradeOption>();

        // Limit to the number of available options, max 3
        int numberOfUpgradesToSelect = Mathf.Min(3, availableOptions.Count);

        while (selectedOptions.Count < numberOfUpgradesToSelect && availableOptions.Count > 0)
        {
            int randomIndex = Random.Range(0, availableOptions.Count);
            selectedOptions.Add(availableOptions[randomIndex]);
            availableOptions.RemoveAt(randomIndex);
        }

        Debug.Log($"Selected Options Count: {selectedOptions.Count}");
        for (int i = 0; i < selectedOptions.Count; i++)
        {
            Debug.Log($"Option {i}: {selectedOptions[i].upgradeName}, Level: {selectedOptions[i].currentLevel}");
        }

        return selectedOptions;
    }


    private bool AreAllUpgradesMaxLevel()
    {
        bool allMax = playerShooting.volleyLevel >= 5 &&
                      bubbleShield.currentAuraLevel >= 5 &&
                      spark.sparkLevel >= 5 &&
                      ricochet.currentRicochetLevel >= 5 &&
                      playerScript.globalDamageLevel >= 5 &&
                      playerScript.currentArmorLevel >= 5 &&
                      playerScript.currentMoveSpeedLevel >= 5;

        Debug.Log($"Are all upgrades max level? {allMax}");
        return allMax;
    }

    private void ApplySelectedUpgrade(int index, List<UpgradeOption> selectedUpgrades)
    {
        if (index < 0 || index >= selectedUpgrades.Count) return;

        Debug.Log($"Applying upgrade: {selectedUpgrades[index].upgradeName} at index {index}");
        selectedUpgrades[index].upgradeAction.Invoke();
        HideUpgradePanel();
    }

    public void UpdateUpgradeLevel(string upgradeName, int newLevel)
    {
        UpgradeOption upgrade = availableUpgrades.Find(u => u.upgradeName == upgradeName);
        if (upgrade != null)
        {
            upgrade.currentLevel = newLevel;
            Debug.Log($"{upgradeName} level updated to: {newLevel}");
        }
        else
        {
            Debug.LogWarning($"Upgrade {upgradeName} not found in available upgrades.");
        }
    }
    private void ApplyVolleyUpgrade()
    {
        playerShooting.IncreaseVolleyLevel();
    }

    private void ApplyBubbleShieldUpgrade()
    {
        bubbleShield.IncreaseAuraLevel();
    }

    private void ApplySparkUpgrade()
    {
        spark.IncreaseSparkLevel();
    }

    private void ApplyRicochetUpgrade()
    {
        ricochet.IncreaseRicochetLevel();
    }

    private void ApplyMoveSpeedUpgrade()
    {
        playerScript.IncreaseMoveSpeedLevel();
    }
    private void ApplyArmorUpgrade()
    {
        playerScript.IncreaseArmorLevel();
    }
    private void ApplyGlobalDamageUpgrade()
    {
        playerScript.IncreaseGlobalDamageLevel();
    }
}
