using TMPro;
using UnityEngine;

public class AchievementMenu : MonoBehaviour
{
    public TextMeshProUGUI projectilesFiredText;
    public TextMeshProUGUI enemiesKilledText;
    public TextMeshProUGUI miniBossesKilledText;
    public TextMeshProUGUI bossesKilledText;
    public TextMeshProUGUI distanceTraveledText;
    public TextMeshProUGUI xpGainedText;
    public TextMeshProUGUI coinsGainedText;

    private int projectilesFired;
    private int enemiesKilled;
    private int miniBossesKilled;
    private int bossesKilled;
    private float totalDistanceTraveled;
    private float totalXPGained;
    private int totalCoinsGained;

    void Start()
    {
        LoadAchievements();
        UpdateUI();
    }

    // Load stats from PlayerPrefs
    private void LoadAchievements()
    {
        projectilesFired = PlayerPrefs.GetInt("ProjectilesFired", 0);  // Default to 0 if no data exists
        enemiesKilled = PlayerPrefs.GetInt("EnemiesKilled", 0);  // Default to 0
        miniBossesKilled = PlayerPrefs.GetInt("MiniBossesKilled", 0);  // Default to 0
        bossesKilled = PlayerPrefs.GetInt("BossesKilled", 0);  // Default to 0
        totalDistanceTraveled = PlayerPrefs.GetFloat("TotalDistanceTraveled", 0f);
        totalXPGained = PlayerPrefs.GetFloat("TotalXPGained", 0f);
        totalCoinsGained = PlayerPrefs.GetInt("TotalCoinsGained", 0);
    }

    // Update the visual UI with loaded stats
    private void UpdateUI()
    {
        projectilesFiredText.text = "Projectiles Fired: " + projectilesFired;
        enemiesKilledText.text = "Enemies Killed: " + enemiesKilled;
        miniBossesKilledText.text = "MiniBosses Killed: " + miniBossesKilled;
        bossesKilledText.text = "Bosses Killed: " + bossesKilled;
        distanceTraveledText.text = "Total Distance Traveled: " + totalDistanceTraveled.ToString("F2") + " units";
        xpGainedText.text = "XP Gained: " + totalXPGained;
        coinsGainedText.text = "Coins Gained: " + totalCoinsGained;

    }

    // Optionally, save achievements when you want to persist data
    private void SaveAchievements()
    {
        PlayerPrefs.SetInt("ProjectilesFired", projectilesFired);
        PlayerPrefs.SetInt("EnemiesKilled", enemiesKilled);
        PlayerPrefs.SetInt("MiniBossesKilled", miniBossesKilled);
        PlayerPrefs.SetInt("BossesKilled", bossesKilled);
        PlayerPrefs.SetFloat("TotalDistanceTraveled", totalDistanceTraveled);
        PlayerPrefs.SetFloat("TotalXPGained", totalXPGained);
        PlayerPrefs.SetInt("TotalCoinsGained", totalCoinsGained);
        PlayerPrefs.Save();
    }
}
