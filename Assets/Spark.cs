using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spark : MonoBehaviour
{
    public GameObject sparkProjectilePrefab;     // Prefab for the spark projectile
    private float baseDamage = 1f;                // Base damage dealt by the spark projectile
    private float baseSpeed = 10f;                 // Base speed of the spark projectile
    private float baseFireRate;               // Base time between shots (lower = faster fire rate)
    private float nextFireTime = 0f;              // When the next projectile can be fired
    private Transform target;                      // Target enemy to shoot at
    public float detectionRadius = 5f;            // Radius within which the projectile will detect enemies
    public int sparkLevel = 0;                    // Tracks the spark upgrade level (0 to 5)
    private UpgradeManager upgradeManager;  // Reference to UpgradeManager
    private player_script playerScript;   // Reference to the player script to access global damage
    private int projectilesFired;


    void Start()
    {
        projectilesFired = PlayerPrefs.GetInt("ProjectilesFired", 0);  // Load saved projectiles fired count
        upgradeManager = FindObjectOfType<UpgradeManager>();
        playerScript = GetComponent<player_script>(); // Get the PlayerScript component

    }
    void Update()
    {
        // Find the nearest enemy
        FindNearestEnemy();

        // Check if there is a target and if it's time to shoot
        if (target != null && Time.time >= nextFireTime)
        {
            ShootAtTarget();
            nextFireTime = Time.time + GetCurrentFireRate();  // Update next fire time
        }
    }

    private void FindNearestEnemy()
    {
        // Debug.Log("Find Nearest Enemy Start");
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, detectionRadius, LayerMask.GetMask("Enemy"));
        float closestDistance = Mathf.Infinity;

        target = null; // Reset target

        // Find the closest enemy
        foreach (var enemyCollider in enemiesInRange)
        {
            float distance = Vector2.Distance(transform.position, enemyCollider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                target = enemyCollider.transform;
            }
        }
    }

    private void ShootAtTarget()
    {
        if (target != null && sparkProjectilePrefab != null && sparkLevel >= 1)
        {
            // Calculate direction to the target
            Vector3 direction = (target.position - transform.position).normalized;

            // Instantiate the projectile
            GameObject projectile = Instantiate(sparkProjectilePrefab, transform.position, Quaternion.identity);
            projectile.layer = LayerMask.NameToLayer("Projectile");

            // Set the rotation of the projectile to face the target
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; // Convert from radians to degrees
            projectile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            // Get the Rigidbody2D and set its velocity
            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction * GetCurrentSpeed();
            }

            // Set the damage for the projectile
            Projectile projectileComponent = projectile.GetComponent<Projectile>();
            if (projectileComponent != null)
            {
                projectileComponent.SetDamage(GetCurrentDamage() * playerScript.globalDamageMultiplier * playerScript.baseDamageMultiplier);
            }
            IncrementProjectileCount();
            // Destroy the spark projectile after a certain time (optional)
            Destroy(projectile, 5f);  // Change 5f to your desired lifespan
        }
    }

    public void SetDamage(float newDamage)
    {
        baseDamage = newDamage;
    }

    public void SetSpeed(float newSpeed)
    {
        baseSpeed = newSpeed;
    }

    public void SetFireRate(float newFireRate)
    {
        baseFireRate = newFireRate;
    }

    public void IncreaseSparkLevel()
    {
        if (sparkLevel < 5) // Max level is 5
        {
            sparkLevel++;
            UpdateSparkStats();  // Update aura's stats after leveling up
            upgradeManager.UpdateUpgradeLevel("Spark", sparkLevel); // Update the UpgradeManager
            Debug.Log("Spark level increased to: " + sparkLevel);

        }
        else
        {
            Debug.Log("Spark is already at max level.");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collided object is an enemy
        if (other.CompareTag("Enemy") || other.CompareTag("Boss") || other.CompareTag("MiniBoss"))
        {
            // Assuming the enemy has a Health component
            Health enemyHealth = other.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(GetCurrentDamage()); // Call method to deal damage to the enemy
            }
        }
    }

    private void UpdateSparkStats()
    {
        switch (sparkLevel)
        {
            case 0:
                // Level 0: No shooting
                baseDamage = 0f;
                baseSpeed = 0f;
                baseFireRate = Mathf.Infinity; // Prevent shooting
                break;
            case 1:
                // Level 1: 1 projectile, basic stats
                baseDamage = 1f;
                baseSpeed = 10f;
                baseFireRate = 0.5f;
                break;
            case 2:
                // Level 2: 2 projectiles, slightly improved stats
                baseDamage = 2f;
                baseSpeed = 11.5f;
                baseFireRate = 0.3f;
                break;
            case 3:
                // Level 3: 3 projectiles, moderate improvements
                baseDamage = 3f;
                baseSpeed = 13f;
                baseFireRate = 0.1f;
                break;
            case 4:
                // Level 4: 4 projectiles, significant improvements
                baseDamage = 4f;
                baseSpeed = 14.5f;
                baseFireRate = 0.085f;
                break;
            case 5:
                // Level 5: 5 projectiles, max improvements
                baseDamage = 5f;
                baseSpeed = 16f;
                baseFireRate = 0.06f; // Fastest shooting
                break;
            default:
                break;
        }
    }

    private float GetCurrentDamage()
    {
        return baseDamage;
    }

    private float GetCurrentSpeed()
    {
        return baseSpeed;
    }

    private float GetCurrentFireRate()
    {
        return baseFireRate;
    }

    private void IncrementProjectileCount()
    {
        projectilesFired++;
        PlayerPrefs.SetInt("ProjectilesFired", projectilesFired);
        PlayerPrefs.Save();
    }
}
