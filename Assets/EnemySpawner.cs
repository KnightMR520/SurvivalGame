using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;    // Reference to the enemy prefab
    public GameObject miniBossPrefab;    // Reference to the enemy prefab
    public GameObject bossPrefab;    // Reference to the enemy prefab
    public Transform player;          // Reference to the player's transform
    public float spawnInterval = 0.35f;  // Time between spawns
    public float spawnRadius = 10f;   // Radius around the player to spawn enemies
    public float baseHealth = 10f;   // Base health of enemies
    public float baseSize = 10f;      // Base size of enemies
    public float baseDamage = 10f;    // Base damage of enemies
    private int minuteCount = 0;      // Tracks the number of minutes passed
    public GameTimer gameTimer;       // Reference to the GameTimer script
    private float minibossSpawnInterval = 150; // 2.5 minutes in seconds 150 
    private float ringOfEnemiesSpawnInterval = 90;
    private bool bossSpawned = false;   // Flag to track if the boss has been spawned
    private PlayerCoinManager playerCoinManager;


    // List of colors corresponding to each minute
    private Color[] minuteColors = {
        Color.white,  // Minute 0 (base color)
        Color.green,  // Minute 1
        Color.yellow, // Minute 2
        new Color(1f, 0.65f, 0f), // Orange for Minute 3
        Color.red     // Minute 4
    };

    void Start()
    {
        playerCoinManager = FindObjectOfType<PlayerCoinManager>();
        // Start spawning enemies continuously
        InvokeRepeating("SpawnEnemy", 0f, spawnInterval);
        Invoke("SpawnMiniboss", minibossSpawnInterval); // Start miniboss spawns after 2.5 minutes
        Invoke("SpawnRingOfEnemies", ringOfEnemiesSpawnInterval); // Spawn ring of enemies after 1 minute

    }

    void Update()
    {
        // Access elapsedTime from the GameTimer
        float elapsedTime = gameTimer.elapsedTime;

        // Every minute, increase the enemy health, damage, and change color
        if (elapsedTime >= (minuteCount + 1) * 60f)
        {
            minuteCount++;
            ApplyEnemyModifiers();
        }

        // Boss spawn logic at 20 minutes
        if (elapsedTime >= 1200f && !bossSpawned) // 1200 seconds = 20 minutes
        {
            SpawnBoss();
            bossSpawned = true; // Ensure boss is only spawned once
        }

        if (elapsedTime >= 1200 && GameObject.FindGameObjectWithTag("Boss") == null)
        {
            EndGame();
        }
    }

    void ApplyEnemyModifiers()
    {
        // Increase enemy health by 20% per minute and increase spawn rate by 10%
        baseHealth *= 1.075f;  // Increase base health by 10%
        baseDamage *= 1.075f;  // Increase base damage by 5%
        spawnInterval *= 0.975f;  // Decrease spawn interval by 1% (increases spawn rate) 0.95 for 5% increase 

        // Reset color and size scaling every 5 minutes, but increase base stats
        if (minuteCount % 5 == 0)
        {
            baseHealth *= 1.15f;  // Increase base health by 20% every 5 minutes
            baseSize *= 1.05f;    // Increase base size by 5% every 5 minutes
        }

        // Update the spawn rate to reflect the new interval
        CancelInvoke("SpawnEnemy");
        InvokeRepeating("SpawnEnemy", 0f, spawnInterval);
    }

    void SpawnEnemy()
    {
        // Generate a random angle to spawn the enemy
        float angle = Random.Range(0f, 360f); // Random angle in degrees

        // Calculate the spawn position based on the angle and spawn radius
        Vector2 spawnPosition = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * spawnRadius;

        // Set the enemy position to be at the maximum radius from the player
        Vector3 enemyPosition = player.position + new Vector3(spawnPosition.x, spawnPosition.y, 0);

        // Spawn the enemy at the generated position
        GameObject newEnemy = Instantiate(enemyPrefab, enemyPosition, Quaternion.identity);

        // Assign the player reference to the enemy's movement script
        newEnemy.GetComponent<EnemyMovement>().player = player;

        // Apply color based on current minute
        int colorIndex = minuteCount % 5; // Wrap around every 5 minutes
        newEnemy.GetComponent<SpriteRenderer>().color = minuteColors[colorIndex];

        // Apply health and size modifiers
        Health enemyHealth = newEnemy.GetComponent<Health>();
        enemyHealth.maxHealth = baseHealth;
        enemyHealth.currentHealth = enemyHealth.maxHealth; // Start with full health

        float enemySize = baseSize; // Size increases every 5 minutes
        newEnemy.transform.localScale = new Vector3(enemySize, enemySize, 1f);
    }

    void SpawnRingOfEnemies()
    {
        int ringSize = 100; // Number of enemies in the ring
        float ringRadius = 15f; // Distance from the player to spawn the ring of enemies
        float increasedHealth = baseHealth * 20f; // Increased health for the ring enemies
        float originalBaseDamage = baseDamage;
        baseDamage *= 1.5f; // Ring enemies deal 50% more damage


        for (int i = 0; i < ringSize; i++)
        {
            // Generate a random angle to spawn the enemy
            float angle = i * (360f / ringSize); // Evenly spaced around the circle

            // Calculate the spawn position based on the angle and spawn radius
            Vector2 spawnPosition = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * ringRadius;

            // Set the enemy position to be at the maximum radius from the player
            Vector3 enemyPosition = player.position + new Vector3(spawnPosition.x, spawnPosition.y, 0);

            // Spawn the enemy at the generated position
            GameObject ringEnemy = Instantiate(enemyPrefab, enemyPosition, Quaternion.identity);

            // Assign the player reference to the enemy's movement script
            ringEnemy.GetComponent<EnemyMovement>().player = player;


            // Set increased health for the ring enemy
            Health ringEnemyHealth = ringEnemy.GetComponent<Health>();
            ringEnemyHealth.maxHealth = increasedHealth;
            ringEnemyHealth.currentHealth = ringEnemyHealth.maxHealth; // Start with full health


            // Set slow movement speed for the ring enemy
            EnemyMovement enemyMovement = ringEnemy.GetComponent<EnemyMovement>();
            enemyMovement.moveSpeed *= 0.05f; // Adjust slow movement speed (change the multiplier as needed)

            // Apply color based on current minute
            ringEnemy.GetComponent<SpriteRenderer>().color = Color.cyan; // You can choose a specific color for ring enemies

            // Scale the enemy if necessary
            ringEnemy.transform.localScale = new Vector3(baseSize, baseSize, 1f);

            // Assign the new layer to the ring enemy
            ringEnemy.layer = LayerMask.NameToLayer("RingEnemies"); // Set the layer to RingEnemies

            // Start the coroutine to destroy the ring enemy after 60 seconds
            StartCoroutine(DestroyRingEnemyAfterTime(ringEnemy, ringEnemyHealth, 40f));

        }
        baseDamage = originalBaseDamage;
        Invoke("SpawnRingOfEnemies", ringOfEnemiesSpawnInterval); // Spawn ring of enemies after 1 minute

    }

    private IEnumerator DestroyRingEnemyAfterTime(GameObject enemy, Health enemyHealth, float time)
    {
        yield return new WaitForSeconds(time);

        if (enemy != null)
        {
            // Call the Die method on the enemy's Health component
            if (enemyHealth != null)
            {
                enemyHealth.Die(); // This will destroy both the enemy and its health bar
            }
        }
    }

    void SpawnMiniboss()
    {
        // Generate a random angle to spawn the miniboss enemy
        float angle = Random.Range(0f, 360f); // Random angle in degrees

        // Calculate the spawn position based on the angle and spawn radius
        Vector2 spawnPosition = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * spawnRadius;

        // Set the miniboss position to be at the maximum radius from the player
        Vector3 minibossPosition = player.position + new Vector3(spawnPosition.x, spawnPosition.y, 0);

        // Spawn the miniboss enemy at the generated position
        GameObject minibossEnemy = Instantiate(miniBossPrefab, minibossPosition, Quaternion.identity);
        minibossEnemy.tag = "MiniBoss"; // Assign a tag for the boss to identify later

        // Assign the player reference to the miniboss's movement script
        minibossEnemy.GetComponent<EnemyMovement>().player = player;

        // Apply a distinct color to the miniboss
        minibossEnemy.GetComponent<SpriteRenderer>().color = Color.blue; // Miniboss color

        // Apply miniboss stats: 5x health, size, and damage of regular enemies
        Health minibossHealth = minibossEnemy.GetComponent<Health>();
        minibossHealth.maxHealth = baseHealth * 125f; // Miniboss has 5x the health
        minibossHealth.currentHealth = minibossHealth.maxHealth; // Start with full health

        float minibossSize = baseSize * 1.75f; // Miniboss is 25% larger than regular enemies
        minibossEnemy.transform.localScale = new Vector3(minibossSize, minibossSize, 1f);

        // Schedule the next miniboss spawn after 2.5 minutes
        Invoke("SpawnMiniboss", minibossSpawnInterval);
    }
    void SpawnBoss()
    {
        // Generate a random angle to spawn the boss enemy
        float angle = Random.Range(0f, 360f); // Random angle in degrees

        // Calculate the spawn position based on the angle and spawn radius
        Vector2 spawnPosition = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * spawnRadius;

        // Set the boss position to be at the maximum radius from the player
        Vector3 bossPosition = player.position + new Vector3(spawnPosition.x, spawnPosition.y, 0);

        // Spawn the boss enemy at the generated position
        GameObject bossEnemy = Instantiate(enemyPrefab, bossPosition, Quaternion.identity);
        bossEnemy.tag = "Boss"; // Assign a tag for the boss to identify later

        // Assign the player reference to the boss's movement script
        bossEnemy.GetComponent<EnemyMovement>().player = player;

        // Apply a distinct color to the boss
        bossEnemy.GetComponent<SpriteRenderer>().color = Color.magenta;  // Boss color

        // Apply boss stats: 10x health, size, and damage of regular enemies
        Health bossHealth = bossEnemy.GetComponent<Health>();
        bossHealth.maxHealth = baseHealth * 250f; // Boss has 100x the health
        bossHealth.currentHealth = bossHealth.maxHealth; // Start with full health

        float bossSize = baseSize * 2.25f; // Boss is 75% larger than regular enemies
        bossEnemy.transform.localScale = new Vector3(bossSize, bossSize, 1f);
    }

    public void EndGame()
    {
        // Implement your game ending logic here
        // For example, you can load a different scene or display a game over screen
        playerCoinManager.SaveCoins();
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
        // SceneManager.LoadScene("GameOverScene"); // Uncomment and set your Game Over scene name

    }
}
