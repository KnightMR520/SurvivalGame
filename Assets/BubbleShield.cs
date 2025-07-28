using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleShield : MonoBehaviour
{
    public float baseRadius = 0.5f;          // Base radius of the aura
    public float baseSlowPercent = 0.1f;     // Base slow percentage (10% slow at level 1)
    public int currentAuraLevel = 0;         // Start at level 0, meaning no slow effect
    public GameObject auraVisual;
    private CircleCollider2D auraCollider;   // Collider that represents the aura's area of effect
    private UpgradeManager upgradeManager;  // Reference to UpgradeManager


    void Start()
    {
        upgradeManager = FindObjectOfType<UpgradeManager>();
        // Use the existing trigger collider from the AuraVisual GameObject
        auraCollider = auraVisual.GetComponent<CircleCollider2D>();

        if (auraCollider == null)
        {
            Debug.LogError("No Collider found on AuraVisual.");
            return;
        }

        if (!auraCollider.isTrigger)
        {
            auraCollider.isTrigger = true;  // Ensure it's set as a trigger
        }

        UpdateAuraStats();  // Set the initial radius and slow effect
    }

    void Update()
    {
        // Ensure the aura visual follows the player
        if (auraVisual != null)
        {
            auraVisual.transform.position = new Vector3(transform.position.x, transform.position.y, 1.5f); // Adjust Z for correct rendering order
        }
    }

    public void IncreaseAuraLevel()
    {
        if (currentAuraLevel < 5) // Max level is 5
        {
            currentAuraLevel++;
            upgradeManager.UpdateUpgradeLevel("Bubble Shield", currentAuraLevel); // Update the UpgradeManager
            UpdateAuraStats();  // Update aura's stats after leveling up
        }
    }

    private void UpdateAuraStats()
    {
        float newRadius;
        float slowEffect;

        // No slow effect at level 0
        if (currentAuraLevel == 0)
        {
            newRadius = 0f;  // Disable aura effect
            slowEffect = 0f;  // No slowing at level 0
        }
        else
        {
            // Set radius and slow effect based on current level
            newRadius = baseRadius * (1 + 0.1f * (currentAuraLevel - 1));
            slowEffect = baseSlowPercent * (1 + 0.1f * (currentAuraLevel - 1));  // Increase slow effect by 10% per level
        }

        // Update the visual's scale to match the radius
        if (auraVisual != null)
        {
            float visualScale = newRadius * 10;  // Multiply by 2 to match the diameter
            auraVisual.transform.localScale = new Vector3(visualScale, visualScale, 1.5f);
        }

        Debug.Log($"Aura Level: {currentAuraLevel}, Radius: {newRadius}, Slow Effect: {slowEffect * 100}%");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Triggered by something - BubbleShield");
        if (currentAuraLevel > 0 && (other.CompareTag("Enemy") || other.CompareTag("Boss") || other.CompareTag("MiniBoss")))  // Only slow if aura level is greater than 0
        {
            EnemyMovement enemy = other.GetComponent<EnemyMovement>();
            if (enemy != null)
            {
                ApplySlowEffect(enemy);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (currentAuraLevel > 0 && (other.CompareTag("Enemy") || other.CompareTag("Boss") || other.CompareTag("MiniBoss")))
        {
            EnemyMovement enemy = other.GetComponent<EnemyMovement>();
            if (enemy != null)
            {
                RemoveSlowEffect(enemy);
            }
        }
    }

    private void ApplySlowEffect(EnemyMovement enemy)
    {
        // Debug.Log("Applying Slow - BubbleShield");
        float slowModifier = baseSlowPercent * (1 + 0.1f * (currentAuraLevel - 1));  // Get the slow percentage for the current level
        enemy.ApplySlow(slowModifier);
    }

    private void RemoveSlowEffect(EnemyMovement enemy)
    {
        enemy.RemoveSlow();  // Reset enemy speed to its normal value
    }
}