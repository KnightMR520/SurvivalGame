using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public Transform player;  // Reference to the player's transform
    public float moveSpeed = 0.75f;  // Normal enemy speed
    private float currentSpeed;   // Actual speed affected by aura
    private bool facingRight = true;  // Track the current facing direction
    private Rigidbody2D rb;  // Rigidbody2D for physics-based movement
    public float pushForce = 100f;  // Force applied when the player pushes the enemy
    public float mass = 0.0001f;       // Mass of the enemy, make this low to allow easier pushing

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();  // Get the Rigidbody2D component
        currentSpeed = moveSpeed; // Initially, current speed is the normal speed
        rb.mass = mass;  // Set the enemy's mass
        rb.drag = 1.5f;  // Optional: Increase drag to make the enemy "heavier" and slow down after pushing
    }

    void Update()
    {
        if (player == null)
        {
            return;  // In case player reference is missing
        }

        MoveTowardsPlayer();
        FlipSpriteTowardsPlayer();
    }

    // Method to apply the slow effect
    public void ApplySlow(float slowPercent)
    {
        // Debug.Log("Slow Applied - Enemy");
        currentSpeed = moveSpeed * (1f - slowPercent);  // Reduce speed by the slow percentage
    }

    // Method to remove the slow effect (reset speed to normal)
    public void RemoveSlow()
    {
        currentSpeed = moveSpeed; // Reset current speed to original speed
    }

    // Move the enemy towards the player using Rigidbody2D
    void MoveTowardsPlayer()
    {
        // Calculate the direction to the player
        Vector2 direction = (player.position - transform.position).normalized;

        // Set the enemy's velocity towards the player
        rb.velocity = direction * currentSpeed; // Use currentSpeed instead of moveSpeed
    }

    // Flip the enemy's sprite based on movement direction
    void FlipSpriteTowardsPlayer()
    {
        // Calculate the direction and flip the enemy sprite accordingly
        if (rb.velocity.x > 0 && !facingRight)
        {
            Flip();
        }
        else if (rb.velocity.x < 0 && facingRight)
        {
            Flip();
        }
    }

    // Function to flip the enemy's sprite
    void Flip()
    {
        facingRight = !facingRight;  // Switch the direction
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;  // Flip the x-axis scale
        transform.localScale = theScale;
    }

    // Handle collision with the player
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {

            // Calculate direction away from the player
            Vector2 pushDirection = (transform.position - collision.transform.position).normalized;

            // Apply a force to the enemy in the direction away from the player
            rb.AddForce(pushDirection * pushForce, ForceMode2D.Impulse);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        // Reset movement when exiting a collision, if needed
        if (collision.gameObject.CompareTag("Player"))
        {
        }
    }
}
