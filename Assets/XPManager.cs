using System.Collections;
using UnityEngine;

public class XPManager : MonoBehaviour
{
    public static XPManager Instance; // Singleton instance

    public float baseXP = 100f; // Base XP for the first level
    public float currentXP = 0f; // Current XP of the player
    private RectTransform xpBarFill; // Reference to the XP bar fill
    public int currentLevel = 1; // Player's current level
    private float maxXP; // Maximum XP needed for leveling up, will be updated each level
    public UpgradeManager upgradeManager; // Reference to the UpgradeManager
    private bool isLevelingUp = false; // Prevent multiple level-ups at once
    private float totalXPGained = 0f;  // Track the total XP gained


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Set this as the instance
            Debug.Log("XPManager instance created.");
        }
        else
        {
            Debug.LogWarning("Duplicate XPManager instance found and destroyed.");
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    private void Start()
    {
        totalXPGained = PlayerPrefs.GetFloat("TotalXPGained", 0f);

        RectTransform fillTransform = GameObject.Find("XPBarFill").GetComponent<RectTransform>();
        SetXPBarFill(fillTransform);

        xpBarFill.localScale = new Vector3(0, 1, 1); // Set to zero scale on X

        maxXP = baseXP;
    }

    public void AddXP(float amount)
    {
        currentXP += amount;
        totalXPGained += amount;  // Accumulate total XP gained

        UpdateXPBar(); // Update the UI
        Debug.Log($"Current XP: {currentXP} / {maxXP}");

        // Check for leveling up, but only if not already leveling up
        if (currentXP >= maxXP && !isLevelingUp)
        {
            StartCoroutine(LevelUp()); // Call LevelUp as a coroutine to allow yielding
        }

        // Save the total XP gained to PlayerPrefs
        PlayerPrefs.SetFloat("TotalXPGained", totalXPGained);
        PlayerPrefs.Save();
    }

    private IEnumerator LevelUp()
    {
        isLevelingUp = true; // Set flag to true to prevent multiple level-ups
        upgradeManager.ShowUpgradePanel();

        currentXP -= maxXP; // Carry over excess XP to the next level
        currentLevel++; // Increment the player's level

        // Increase the XP required for the next level by 10%
        maxXP = baseXP * Mathf.Pow(1.1f, currentLevel - 1);

        Debug.Log($"Leveled up! Current Level: {currentLevel}, XP for next level: {maxXP}");
        UpdateXPBar(); // Update the XP bar after leveling up

        yield return new WaitForSeconds(0.5f); // Small delay to avoid rapid level-ups

        isLevelingUp = false; // Reset the flag after the level-up completes
    }

    private void UpdateXPBar()
    {
        float fillAmount = currentXP / maxXP;
        xpBarFill.localScale = new Vector3(fillAmount, 1, 1); // Adjust the fill amount
    }

    public void SetXPBarFill(RectTransform fillTransform)
    {
        xpBarFill = fillTransform; // Assign the XP bar fill reference
    }
}

