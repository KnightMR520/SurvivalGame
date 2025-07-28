using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public float attractionRadius = 5f;    // How close the player needs to be before the coin is attracted
    public float baseAttractionSpeed = 5f;  // Initial speed of the coin
    public float maxAttractionSpeed = 20f;  // Maximum speed of the coin
    public float accelerationRate = 2f;      // Rate at which the speed increases
    private Transform player;               // Reference to the player's transform
    private bool isCollected = false;       // Track if the coin has been collected
    private float currentAttractionSpeed;   // Current speed of the coin
    private bool isAttracted = false;       // Track if the coin is attracted to the player
    private Collider2D coinCollider;        // Reference to the coin's collider

    // The amount of coins this object is worth
    public int coinValue = 1;

    // Reference to the player's coin counter (assume this is managed somewhere in PlayerCoinManager)
    private PlayerCoinManager playerCoinManager;

    void Start()
    {
        // Find the player by tag (make sure your player is tagged "Player")
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // Lock the coin's Z position
        transform.position = new Vector3(transform.position.x, transform.position.y, 1);

        // Initialize the current attraction speed
        currentAttractionSpeed = baseAttractionSpeed;

        // Get the coin's collider
        coinCollider = GetComponent<Collider2D>();

        // Find the PlayerCoinManager in the scene
        playerCoinManager = FindObjectOfType<PlayerCoinManager>();
    }

    void Update()
    {
        if (isCollected)
        {
            return; // If collected, do not do anything else
        }

        // Calculate distance between player and coin
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Check if the player is within the attraction radius
        if (distanceToPlayer < attractionRadius)
        {
            // Move the coin towards the player
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

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object that entered the trigger is the player
        if (other.CompareTag("Player") && !isCollected)
        {
            CollectCoin(); // Call CollectCoin to ensure the coin is only collected once
        }
    }

    void CollectCoin()
    {
        if (!isCollected) // Check if already collected
        {
            isCollected = true; // Prevent multiple collection
            Debug.Log("Coin Collected!");

            // Disable the collider and set the coin inactive to prevent further collection
            coinCollider.enabled = false;
            gameObject.SetActive(false); // Immediately disable the coin

            // Add coins to the player's coin count
            if (playerCoinManager != null)
            {
                playerCoinManager.AddCoins(coinValue);
            }

            // Destroy the coin after a short delay
            Destroy(gameObject);
        }
    }
}
