using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ricochet : MonoBehaviour
{
    public int currentRicochetLevel = 0;   // Ricochet level (starts at 0)
    public GameObject ricochetProjectilePrefab;  // Prefab for the ricochet projectile
    public float baseDamage = 1f;   // Base damage for ricochet
    public float baseDuration = 3f;  // Duration before the projectile is destroyed
    public float radius = 5f;        // Radius to search for enemies
    public float fireCooldown = 5f;  // Time between ricochet attacks
    private float lastFireTime = 0f;
    private UpgradeManager upgradeManager;  // Reference to UpgradeManager
    private player_script playerScript;   // Reference to the player script to access global damage
    private int projectilesFired;


    void Start()
    {
        projectilesFired = PlayerPrefs.GetInt("ProjectilesFired", 0);  // Load saved projectiles fired count
        upgradeManager = FindObjectOfType<UpgradeManager>();
        playerScript = GetComponent<player_script>(); // Get the PlayerScript component
        lastFireTime = Time.time - fireCooldown;  // Initialize so it can fire immediately
    }

    void Update()
    {
        if (currentRicochetLevel > 0 && Time.time - lastFireTime >= fireCooldown)
        {
            FireRicochetProjectile();  // Fire ricochet attack if cooldown allows
            lastFireTime = Time.time;
        }
    }

    public void IncreaseRicochetLevel()
    {
        if (currentRicochetLevel < 5)  // Max level is 5
        {
            currentRicochetLevel++;
            upgradeManager.UpdateUpgradeLevel("Ricochet", currentRicochetLevel); // Update the UpgradeManager

        }
    }

    void FireRicochetProjectile()
    {
        // Find the nearest enemy within the radius
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, radius);
        List<GameObject> validTargets = new List<GameObject>();

        foreach (var enemy in enemiesInRange)
        {
            if (enemy.CompareTag("Enemy") || enemy.CompareTag("Boss") || enemy.CompareTag("MiniBoss"))
            {
                validTargets.Add(enemy.gameObject);
            }
        }

        if (validTargets.Count > 0)
        {
            GameObject firstTarget = validTargets[Random.Range(0, validTargets.Count)];
            GameObject ricochetProjectile = Instantiate(ricochetProjectilePrefab, transform.position, Quaternion.identity);
            RicochetProjectile projectileScript = ricochetProjectile.GetComponent<RicochetProjectile>();

            if (projectileScript != null)
            {
                projectileScript.Initialize(firstTarget, GetCurrentDamage() * playerScript.globalDamageMultiplier * playerScript.baseDamageMultiplier, GetCurrentDuration(), radius);
                IncrementProjectileCount();
            }
        }
    }

    float GetCurrentDamage()
    {
        return baseDamage * (1 + (currentRicochetLevel - 1) * 0.5f);  // Increase damage by 50% per level
    }

    float GetCurrentDuration()
    {
        return baseDuration * (1 + (currentRicochetLevel - 1) * 0.2f);  // Increase duration by 20% per level
    }
    private void IncrementProjectileCount()
    {
        projectilesFired++;
        PlayerPrefs.SetInt("ProjectilesFired", projectilesFired);
        PlayerPrefs.Save();
    }
}
