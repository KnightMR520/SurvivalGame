using UnityEngine;

public class TreasureChest : MonoBehaviour
{
    public int coinReward = 100;  // Number of coins the player gets
    private UpgradeManager upgradeManager;  // Reference to the UpgradeManager
    private PlayerCoinManager playerCoinManager;  // Reference to PlayerCoinManager

    void Start()
    {
        // Find the PlayerCoinManager in the scene
        playerCoinManager = FindObjectOfType<PlayerCoinManager>();

        // Ensure UpgradeManager is assigned
        if (upgradeManager == null)
        {
            upgradeManager = FindObjectOfType<UpgradeManager>();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Reward the player with 100 coins
            if (playerCoinManager != null)
            {
                playerCoinManager.AddCoins(coinReward);
            }

            // Trigger the upgrade panel without affecting XP
            if (upgradeManager != null)
            {
                upgradeManager.ShowUpgradePanel();
            }

            // Destroy the chest after it's opened
            Destroy(gameObject);
        }
    }
}
