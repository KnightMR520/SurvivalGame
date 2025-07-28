using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    public GameObject healthBarPrefab; // Reference to the health bar prefab
    private GameObject healthBarInstance; // The instantiated health bar
    private RectTransform healthBarForeground; // The health bar's transform
    private Transform healthBarTransform;  // Transform of the health bar
    public float healAmount = 10f;  // The amount of health to heal
    public float healInterval = 10f;  // Time between healing (10 seconds)
    public GameObject gemPrefab;
    public GameObject coinPrefab;
    public GameObject treasureChestPrefab;
    private bool isDead = false; // Flag to prevent multiple deaths
    private PlayerCoinManager playerCoinManager;
    private GemSpawner gemSpawner;

    void Start()
    {
        playerCoinManager = FindObjectOfType<PlayerCoinManager>();
        gemSpawner = FindObjectOfType<GemSpawner>();

        UpdateMaxHealth();
        // Initialize health
        currentHealth = maxHealth;

        // Instantiate the health bar prefab
        if (healthBarPrefab != null)
        {
            healthBarInstance = Instantiate(healthBarPrefab, transform.position + new Vector3(0, 1.5f, 0), Quaternion.identity);
            healthBarTransform = healthBarInstance.transform;

            // Find the foreground of the health bar by name (or tag)
            healthBarForeground = healthBarInstance.transform.Find("Foreground").GetComponent<RectTransform>();

            // Parent the health bar to the world (not the player/enemy to avoid rotation)
            healthBarTransform.SetParent(null); // Keep it separate from the character
        }

        // Start healing every `healInterval` seconds
        InvokeRepeating("Heal", healInterval, healInterval);
    }

    void Update()
    {
        // Keep the health bar above the enemy and fixed in rotation
        if (healthBarTransform != null)
        {
            healthBarTransform.position = transform.position + new Vector3(0, 1.5f, 0); // Adjust height
            healthBarTransform.rotation = Quaternion.identity; // Keep the rotation fixed
        }
    }

    private void UpdateMaxHealth()
    {
        // Check if this object is the player
        if (gameObject.CompareTag("Player"))
        {
            // Get the player's health upgrade level from PlayerPrefs
            int healthLevel = PlayerPrefs.GetInt("HealthLevel", 0);

            // Apply the health upgrade by increasing maxHealth
            maxHealth += healthLevel * 10;  // Each level increases max health by 10
        }
    }

    // Call this method to reduce health
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        float healthPercent = currentHealth / maxHealth;
        Debug.Log("Damage taken " + damage);

        // Ensure healthPercent doesn't go below 0
        healthPercent = Mathf.Clamp(healthPercent, 0f, 1f);

        // Update the health bar's foreground scale based on health percentage
        if (healthBarForeground != null)
        {
            healthBarForeground.localScale = new Vector3(healthPercent, 1, 1);
        }

        // Check if the entity's health has reached zero
        if (currentHealth <= 0 && !isDead) // Ensure Die() is only called once
        {
            Die();
        }
    }

    // Method to heal the player
    void Heal()
    {
        // Check if the object is the player
        if (gameObject.CompareTag("Player"))
        {
            // Heal by the specified amount, but don't exceed maxHealth
            currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
            UpdateHealthBar();
        }
    }

    // Update the health bar's scale
    void UpdateHealthBar()
    {
        if (healthBarForeground != null)
        {
            float healthPercent = currentHealth / maxHealth;
            healthBarForeground.localScale = new Vector3(healthPercent, 1, 1);
        }
    }

    public void Die()
    {
        isDead = true; // Mark as dead to prevent multiple calls to Die()

        // If the enemy was taking damage, stop the player's continuous damage
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player_script playerScript = player.GetComponent<player_script>();
            if (playerScript != null)
            {
                playerScript.StopDamageOnEnemyDeath(gameObject); // Notify the player to stop taking damage
            }
        }

        // If this is the player, end the game
        if (gameObject.CompareTag("Player"))
        {
            playerCoinManager.SaveCoins();
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
        }
        else
        {
            // Track enemy/miniboss/boss deaths
            TrackEnemyDeaths();

            // Destroy the health bar along with the enemy
            if (healthBarInstance != null)
            {
                Destroy(healthBarInstance);
            }

            // Check if the enemy is not a RingEnemy before dropping gems
            if (gameObject.layer != LayerMask.NameToLayer("RingEnemies"))
            {
                // Handle miniboss logic separately
                if (gameObject.CompareTag("MiniBoss"))
                {
                    // Drop the treasure chest
                    Instantiate(treasureChestPrefab, transform.position, Quaternion.identity);
                    Debug.Log("Chest Dropped");
                }
                else
                {
                    // 50-50 chance to drop 0 or 1 gem
                    int numGems = Random.Range(0, 2); // 0 = no gem, 1 = spawn 1 gem
                    int numCoins = Random.Range(0, 10); // 50-50 chance for coins as well

                    if (numGems == 1)
                    {
                        // Optionally, you can add some random offset to avoid overlapping with other objects
                        Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
                        // Instantiate one gem at the enemy's position with a slight offset
                        // Instantiate(gemPrefab, transform.position + randomOffset, Quaternion.identity);
                        gemSpawner.SpawnGem(transform.position + randomOffset);
                    }

                    if (numCoins == 0)
                    {
                        // Optionally, you can add some random offset to avoid overlapping with other objects
                        Vector3 randomOffset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
                        // Instantiate one coin at the enemy's position with a slight offset
                        Instantiate(coinPrefab, transform.position + randomOffset, Quaternion.identity);
                    }
                }
            }
            // Destroy the enemy itself
            Destroy(gameObject);

        }
    }

    // Track enemy, miniboss, and boss kills
    private void TrackEnemyDeaths()
    {
        if (gameObject.CompareTag("MiniBoss"))
        {
            int miniBossesKilled = PlayerPrefs.GetInt("MiniBossesKilled", 0);
            miniBossesKilled++;
            PlayerPrefs.SetInt("MiniBossesKilled", miniBossesKilled);
        }
        else if (gameObject.CompareTag("Boss"))
        {
            int bossesKilled = PlayerPrefs.GetInt("BossesKilled", 0);
            bossesKilled++;
            PlayerPrefs.SetInt("BossesKilled", bossesKilled);
        }
        else // Assume this is a regular enemy
        {
            int enemiesKilled = PlayerPrefs.GetInt("EnemiesKilled", 0);
            enemiesKilled++;
            PlayerPrefs.SetInt("EnemiesKilled", enemiesKilled);
        }

        PlayerPrefs.Save();
    }

}

