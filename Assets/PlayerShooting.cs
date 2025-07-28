using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject projectilePrefab;  // The projectile prefab
    public float projectileSpeed = 10f;  // Speed of the projectile
    public float fireRate = 0.25f;        // Time between shots (lower = faster fire rate)
    public float attackSpeedMultiplier;
    private float defaultFireRate;  // Store the default fire rate
    public float projectileSpreadAngle = 10f; // Angle between the projectiles in the volley
    public int volleyLevel = 0;          // Tracks the volley upgrade level (0 to 5)
    public float damage = 5f;            // Public damage value for projectiles
    private float nextFireTime = 0f;     // When the player can fire again
    private Animator animator;            // Reference to the Animator
    private AudioSource audioSource;     // Reference to the AudioSource for shooting sounds
    private bool facingRight = true;      // Track whether the player is facing right
    private UpgradeManager upgradeManager;  // Reference to UpgradeManager
    private player_script playerScript;   // Reference to the player script to access global damage
    public float abilityDuration = 10f;  // Duration of the ability
    public float abilityCooldown = 60f;  // Cooldown time
    private bool isOnCooldown = false;
    private float cooldownTimer = 0f;
    public GameObject abilityIcon;  // Reference to the ability icon in the UI
    public Sprite readySprite;      // Sprite when ability is ready
    public Sprite cooldownSprite;   // Sprite when ability is on cooldown
    private int projectilesFired;

    // Start is called before the first frame update
    void Start()
    {
        projectilesFired = PlayerPrefs.GetInt("ProjectilesFired", 0);  // Load saved projectiles fired count
        animator = GetComponent<Animator>();  // Get the Animator component
        audioSource = GetComponent<AudioSource>();  // Get the AudioSource component
        upgradeManager = FindObjectOfType<UpgradeManager>();
        playerScript = GetComponent<player_script>(); // Get the PlayerScript component
        animator.speed = 2.5f;
        UpdateAttackSpeedMultiplier();
    }

    // Update is called once per frame
    void Update()
    {

        // Ability Activation with Spacebar
        if (Input.GetKeyDown(KeyCode.Space) && !isOnCooldown)
        {
            StartCoroutine(ActivateAttackSpeedBoost());
        }

        // Manage Cooldown
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0)
            {
                isOnCooldown = false;
                abilityIcon.GetComponent<UnityEngine.UI.Image>().sprite = readySprite;  // Change sprite to ready
            }
        }
        // Check if it's time to shoot
        if (Time.time >= nextFireTime)
        {
            // Set the IsShooting trigger in the Animator
            animator.SetTrigger("IsShooting2");

            // Call the Shoot method directly
            Shoot();

            // Play shooting sound
            audioSource.Play();  // Play the sound effect

            // Update the next fire time
            nextFireTime = Time.time + (fireRate * attackSpeedMultiplier);
            //Debug.Log("fire rate" + (fireRate * attackSpeedMultiplier));
        }

        // Flip the player to face the mouse horizontally (left or right)
        FlipPlayerToMouse();
    }

    private void UpdateAttackSpeedMultiplier()
    {
        int attackSpeedLevel = PlayerPrefs.GetInt("AttackSpeedLevel", 0);
        Debug.Log("attack speed level: " + attackSpeedLevel);
        attackSpeedMultiplier = 1f - (attackSpeedLevel * 0.01f);
        Debug.Log("Attack speed mult: " + attackSpeedMultiplier);

    }

    // This method will be called by the Animation Event
    public void Shoot()
    {
        // Find the mouse position in world space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;  // Ensure z-position is 0 for 2D

        // Calculate direction from the player to the mouse
        Vector3 direction = (mousePosition - transform.position).normalized;

        // Set an offset in the direction of the shot
        float spawnDistance = 1f;  // Adjust this value to control how far the projectile spawns in front of the player
        Vector3 spawnPosition = transform.position + (direction * spawnDistance);

        // Shoot projectiles based on the volley level
        ShootVolley(spawnPosition, direction);
    }

    private IEnumerator ActivateAttackSpeedBoost()
    {
        // Activate the ability
        isOnCooldown = true;
        cooldownTimer = abilityCooldown;
        abilityIcon.GetComponent<UnityEngine.UI.Image>().sprite = cooldownSprite;  // Change sprite to cooldown

        // Temporarily increase the fire rate
        defaultFireRate = fireRate;  // Store the default fire rate
        fireRate *= 0.75f;  // Double the attack speed (halve the fire rate)

        // Wait for the ability duration
        yield return new WaitForSeconds(abilityDuration);

        // Revert the attack speed and remove the blue outline
        fireRate = defaultFireRate;

        // Wait for cooldown to finish
        yield return new WaitForSeconds(abilityCooldown - abilityDuration);
    }


    // Method to shoot multiple projectiles in a cone-like spread based on volley level
    private void ShootVolley(Vector3 spawnPosition, Vector3 direction)
    {
        // Calculate the number of projectiles based on volley level
        int projectilesToShoot;

        if (volleyLevel == 0 || volleyLevel == 1)
        {
            projectilesToShoot = 1;  // Levels 0 and 1 = 1 projectile
        }
        else if (volleyLevel == 2 || volleyLevel == 3)
        {
            projectilesToShoot = 2;  // Levels 2 and 3 = 2 projectiles
        }
        else
        {
            projectilesToShoot = 3;  // Levels 4, 5, and 6 = 3 projectiles
        }

        float totalSpread = (projectilesToShoot - 1) * projectileSpreadAngle;
        float startingAngle = -totalSpread / 2;

        // Calculate the damage multiplier based on the volley level (5% more damage per level)
        float volleyDamageMultiplier = 1f + (0.05f * volleyLevel);

        for (int i = 0; i < projectilesToShoot; i++)
        {
            // Calculate angle for each projectile
            float angle = startingAngle + (i * projectileSpreadAngle);
            Quaternion rotation = Quaternion.Euler(0, 0, angle);
            Vector3 directionWithSpread = rotation * direction;

            // Calculate the total damage with global damage modifier and volley level multiplier
            float totalDamage = damage * playerScript.globalDamageMultiplier * playerScript.baseDamageMultiplier * volleyDamageMultiplier;

            // Shoot the projectile and set its damage
            ShootSingleProjectile(spawnPosition, directionWithSpread, totalDamage);
        }
    }


    // Method to shoot a single projectile
    private void ShootSingleProjectile(Vector3 spawnPosition, Vector3 direction, float projectileDamage)
    {
        // Instantiate the projectile at the new spawn position (in front of the player)
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        // Add velocity to the projectile in the direction of the mouse
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        rb.velocity = direction * projectileSpeed;

        // Set the projectile damage using the passed damage value
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript != null)
        {
            projectileScript.SetDamage(projectileDamage);  // Set the damage for this specific projectile
        }
        // Rotate the projectile to face the direction it's moving
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        IncrementProjectileCount();
    }

    // Method to increase the volley level (up to 5)
    public void IncreaseVolleyLevel()
    {
        if (volleyLevel < 5) // Ensure we don't exceed max level
        {
            volleyLevel++;
            upgradeManager.UpdateUpgradeLevel("Volley", volleyLevel); // Update the UpgradeManager
            Debug.Log("Volley level increased to: " + volleyLevel);
        }
        else
        {
            Debug.Log("Volley is already at max level.");
        }
    }

    // Flip the player to face the mouse horizontally
    void FlipPlayerToMouse()
    {
        // Get the mouse position in world space
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Check if the mouse is to the left or right of the player
        bool mouseIsRight = mousePosition.x > transform.position.x;

        // Flip the player based on the mouse position
        if (mouseIsRight && !facingRight)
        {
            Flip();
        }
        else if (!mouseIsRight && facingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        // Flip the player's facing direction
        facingRight = !facingRight;

        // Multiply the player's x local scale by -1 to flip the sprite horizontally
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void IncrementProjectileCount()
    {
        projectilesFired++;
        PlayerPrefs.SetInt("ProjectilesFired", projectilesFired);
        PlayerPrefs.Save();
    }
}
