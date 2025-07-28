using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class player_script : MonoBehaviour
{
    public float globalDamageMultiplier = 1.0f;  // Start with no increase (100% damage)
    public int globalDamageLevel = 0;
    public float baseDamageMultiplier = 1.0f;  // Damage multiplier from upgrades
    public float moveSpeedMultiplier = 1.0f;  // Damage multiplier from upgrades
    public float baseMoveSpeed = 4.5f;  // Movement speed of the player
    public float currentMoveSpeed;   // Actual movement speed after applying upgrades
    public int currentMoveSpeedLevel;  // Multiplier for move speed upgrade
    public int currentArmorLevel;  // Level of armor upgrade
    private float ArmorMultiplier;
    private float baseDamageReduction = 1f;  // Base reduction in damage per armor level
    private Rigidbody2D rb;       // Reference to the player's Rigidbody2D
    private Vector2 movement;     // Store the player's movement direction
    public Health playerHealth;   // Reference to the player's Health script
    private Animator animator;    // Reference to the Animator
    private bool isTakingDamage = false;  // Track if the player is taking continuous damage
    private float damageCooldown = 1f;   // Time between damage ticks (1 second)
    private float lastDamageTime = 0f;   // Tracks the last time damage was applied
    private UpgradeManager upgradeManager;  // Reference to UpgradeManager
    private EnemySpawner enemySpawner;  // Reference to the EnemySpawner script
    // New distance traveled variable
    private float totalDistanceTraveled = 0f;
    private Vector3 lastPosition;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerHealth = GetComponent<Health>();
        animator = GetComponent<Animator>();
        upgradeManager = FindObjectOfType<UpgradeManager>();
        enemySpawner = FindObjectOfType<EnemySpawner>();

        // Initialize the current move speed
        UpdateBaseDamageMultiplier();
        UpdateBaseMovementSpeedMultiplier();
        UpdateArmorMultiplier();

        // Load total distance traveled from PlayerPrefs
        totalDistanceTraveled = PlayerPrefs.GetFloat("TotalDistanceTraveled", 0f);
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Get input from the player
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Update the class-level movement vector
        movement = new Vector2(horizontal, vertical);

        // Normalize the vector to maintain consistent speed if diagonal
        if (movement.magnitude > 1)
        {
            movement.Normalize();
        }

        // Set the Speed parameter in the Animator
        animator.SetFloat("Speed", movement.magnitude);

        // Handle continuous damage if the player is in contact with an enemy
        if (isTakingDamage && Time.time - lastDamageTime >= damageCooldown)
        {
            TakeContinuousDamage();
        }

        // Calculate distance
        float distanceThisFrame = Vector3.Distance(transform.position, lastPosition);
        totalDistanceTraveled += distanceThisFrame;
        lastPosition = transform.position;

        // Save total distance at intervals (e.g., every 5 seconds)
        if (Time.time % 5 == 0)
        {
            PlayerPrefs.SetFloat("TotalDistanceTraveled", totalDistanceTraveled);
            PlayerPrefs.Save();
        }
    }

    void FixedUpdate()
    {
        rb.velocity = movement * currentMoveSpeed * moveSpeedMultiplier;  // Apply multiplier
    }
    public void IncreaseMoveSpeedLevel()
    {
        if (currentMoveSpeedLevel < 5)  // Max level is 5
        {
            currentMoveSpeedLevel++;
            currentMoveSpeed = baseMoveSpeed * (1 + 0.05f * currentMoveSpeedLevel);  // Apply 5% increase per level
            upgradeManager.UpdateUpgradeLevel("Move Speed", currentMoveSpeedLevel); // Update the UpgradeManager
        }
    }
    private void UpdateBaseMovementSpeedMultiplier()
    {
        int moveSpeedLevel = PlayerPrefs.GetInt("SpeedLevel", 0);
        Debug.Log("move speed level: " + moveSpeedLevel);
        moveSpeedMultiplier = 1f + (moveSpeedLevel * 0.01f);

    }
    public void IncreaseArmorLevel()
    {
        if (currentArmorLevel < 5)  // Max level is 5
        {
            currentArmorLevel++;
            upgradeManager.UpdateUpgradeLevel("Armor", currentArmorLevel); // Update the UpgradeManager
        }
    }

    private void UpdateArmorMultiplier()
    {
        int armorLevel = PlayerPrefs.GetInt("ArmorLevel", 0);
        Debug.Log("armor level: " + armorLevel);
        ArmorMultiplier = 1f + (armorLevel * 0.025f);

    }

    // Call this whenever the global damage level increases
    public void IncreaseGlobalDamageLevel()
    {
        if (globalDamageLevel < 5)
        {
            globalDamageLevel++;
            UpdateGlobalDamageMultiplier();
            upgradeManager.UpdateUpgradeLevel("Global Damage", globalDamageLevel);
        }
    }
    // This function updates the multiplier based on the current level
    private void UpdateGlobalDamageMultiplier()
    {
        globalDamageMultiplier = 1f + (globalDamageLevel * 0.1f);
    }

    private void UpdateBaseDamageMultiplier()
    {
        int damageLevel = PlayerPrefs.GetInt("DamageLevel", 0);
        Debug.Log("damage level: " + damageLevel);
        baseDamageMultiplier = 1f + (damageLevel * 0.05f);
    }


    // Method to apply damage with the global multiplier
    public float ApplyGlobalDamage(float baseDamage)
    {
        return baseDamage * globalDamageMultiplier * baseDamageMultiplier;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Collision with {collision.gameObject.name}");
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Boss") || collision.gameObject.CompareTag("MiniBoss"))
        {
            // Start taking continuous damage
            isTakingDamage = true;
            TakeContinuousDamage(); // Immediate damage on first contact
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if ((collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Boss") || collision.gameObject.CompareTag("MiniBoss")) && collision.gameObject.layer != LayerMask.NameToLayer("BubbleShield"))
        {
            // Stop taking continuous damage when the enemy leaves
            isTakingDamage = false;
        }
    }

    // Method to handle continuous damage over time
    void TakeContinuousDamage()
    {
        if (playerHealth != null)
        {
            // Reduce damage based on current armor level and apply the enemy's base damage
            float baseEnemyDamage = enemySpawner.baseDamage;  // Get the current enemy base damage
            int damage = Mathf.Max(0, (int)(baseEnemyDamage - (baseDamageReduction * (currentArmorLevel + 1) * ArmorMultiplier)));
            Debug.Log("Armor Reduction: " + ArmorMultiplier);
            Debug.Log("Base Damage Reduction: " + baseDamageReduction);
            playerHealth.TakeDamage(damage);  // Apply reduced damage
            lastDamageTime = Time.time;   // Reset the damage timer
            Debug.Log($"Player took {damage} damage from enemy after armor reduction!");
        }
    }

    // Stop damage if the enemy dies while the player is in contact
    public void StopDamageOnEnemyDeath(GameObject enemy)
    {
        if (isTakingDamage && enemy.CompareTag("Enemy"))
        {
            isTakingDamage = false;
            Debug.Log("Enemy died, stopping continuous damage.");
        }
    }
}
