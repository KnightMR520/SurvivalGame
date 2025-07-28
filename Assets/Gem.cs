using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    public float attractionRadius = 5f;    // How close the player needs to be before the gem is attracted
    public float baseAttractionSpeed = 5f;  // Initial speed of the gem
    public float maxAttractionSpeed = 20f;  // Maximum speed of the gem
    public float accelerationRate = 2f;      // Rate at which the speed increases
    private Transform player;               // Reference to the player's transform
    private bool isCollected = false;       // Track if the gem has been collected
    private float currentAttractionSpeed;   // Current speed of the gem
    private bool isAttracted = false;       // Track if the gem is attracted to the player
    public float xpValue = 10f; // XP value for this gem
    public XPManager xpManager;  // Reference to the XPManager
    private Collider2D gemCollider;  // Reference to the gem's collider
    private float XPModifier;
    private float baseXPModifier = 1f;

    void Start()
    {
        // Find the player by tag (make sure your player is tagged "Player")
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Lock the gem's Z position
        transform.position = new Vector3(transform.position.x, transform.position.y, 1);

        // Initialize the current attraction speed
        currentAttractionSpeed = baseAttractionSpeed;

        // Get the gem's collider
        gemCollider = GetComponent<Collider2D>();

        UpdateXPMultiplier();
    }

    void Update()
    {
        if (isCollected)
        {
            return; // If collected, do not do anything else
        }

        // Calculate distance between player and gem
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Check if the player is within the attraction radius
        if (distanceToPlayer < attractionRadius)
        {
            // Move the gem towards the player
            transform.position = Vector2.MoveTowards(transform.position, player.position, currentAttractionSpeed * Time.deltaTime);

            // Lock the Z-axis to 1 while moving
            transform.position = new Vector3(transform.position.x, transform.position.y, 1);

            // Increase the attraction speed over time, up to the maximum speed
            currentAttractionSpeed = Mathf.Min(currentAttractionSpeed + accelerationRate * Time.deltaTime, maxAttractionSpeed);
            isAttracted = true; // Mark as attracted to the player
        }
        else if (isAttracted)
        {
            // Reset speed if the player moves away
            currentAttractionSpeed = baseAttractionSpeed;
            isAttracted = false; // Mark as not attracted anymore
        }
    }

    private void UpdateXPMultiplier()
    {
        int XPLevel = PlayerPrefs.GetInt("XPIncreaseLevel", 0);
        Debug.Log("move speed level: " + XPLevel);
        XPModifier = baseXPModifier * (1f + (XPLevel * 0.02f)); // 2% increase per level
    }
    // When the gem collides with the player, start attraction
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            CollectGem(); // Call CollectGem to ensure the gem is only collected once
        }
    }

    void CollectGem()
    {
        if (!isCollected) // Check if already collected
        {
            isCollected = true; // Prevent multiple collection
            Debug.Log("Gem Collected!");

            // Disable the collider and set the gem inactive to prevent further collection
            gemCollider.enabled = false;
            gameObject.SetActive(false); // Immediately disable the gem

            XPManager xpManager = FindObjectOfType<XPManager>(); // Find XPManager in the scene
            if (xpManager != null)
            {
                xpManager.AddXP(xpValue * XPModifier); // Add XP to the player
                Debug.Log("XP: " + xpValue);
                Debug.Log("XPModifier: " + XPModifier);
            }
            else
            {
                Debug.LogError("XPManager not found in the scene!");
            }

            Debug.Log($"Destroying gem {gameObject.name}");

            Destroy(gameObject); // Destroy the gem after a short delay
        }
    }


}
