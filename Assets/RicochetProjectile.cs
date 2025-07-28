using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RicochetProjectile : MonoBehaviour
{
    public float speed = 5f;    // Speed of the projectile
    private float damage;
    private float duration;
    private float remainingDuration;
    private GameObject currentTarget;
    private List<GameObject> hitEnemies = new List<GameObject>();
    private float radius;
    private Rigidbody2D rb;
    private float minDistanceToNextTarget = 2f; // Minimum distance to the next target

    public void Initialize(GameObject initialTarget, float damageAmount, float projectileDuration, float bounceRadius)
    {
        currentTarget = initialTarget;
        damage = damageAmount;
        duration = projectileDuration;
        remainingDuration = duration;
        radius = bounceRadius;

        // Get the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();

        // Ensure the projectile is at z = 1 at the moment of initialization
        transform.position = new Vector3(transform.position.x, transform.position.y, 1f);
        Debug.Log("Projectile Spawn Position: " + transform.position);
    }

    void FixedUpdate()
    {
        // Ensure the projectile stays at z = 1 every frame
        Vector3 currentPosition = transform.position;
        if (currentPosition.z != 1f)
        {
            transform.position = new Vector3(currentPosition.x, currentPosition.y, 1f);
        }

        if (currentTarget != null)
        {
            // Calculate the direction to the current target
            Vector2 direction = (currentTarget.transform.position - transform.position).normalized;

            // Apply force to move towards the target
            rb.velocity = direction * speed;

            // Rotate the projectile to face the direction of movement
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;  // Convert radians to degrees
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            // Check if the projectile reaches the target
            if (Vector2.Distance(transform.position, currentTarget.transform.position) < 0.1f)
            {
                HitEnemy(currentTarget);  // Damage and bounce to the next target
                FindNewTarget();          // Look for a new target after hitting
            }

            // Check for enemies in the projectile's path and damage them
            DealDamageToEnemiesInPath(direction);
        }

        // Decrease the remaining duration and destroy if expired
        remainingDuration -= Time.deltaTime;
        if (remainingDuration <= 0)
        {
            Destroy(gameObject);  // Destroy the projectile after its lifetime expires
        }
    }

    void FindNewTarget()
    {
        // Find another target to bounce to within the radius
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, radius);
        List<GameObject> validTargets = new List<GameObject>();

        foreach (var potentialTarget in enemiesInRange)
        {
            if ((potentialTarget.CompareTag("Enemy") || potentialTarget.CompareTag("Boss") || potentialTarget.CompareTag("MiniBoss")) && !hitEnemies.Contains(potentialTarget.gameObject))
            {
                // Check the distance from the last hit enemy
                if (currentTarget != null && Vector2.Distance(potentialTarget.transform.position, currentTarget.transform.position) >= minDistanceToNextTarget)
                {
                    validTargets.Add(potentialTarget.gameObject);
                }
                else if (currentTarget == null)  // Allow targeting if there was no previous target
                {
                    validTargets.Add(potentialTarget.gameObject);
                }
            }
        }

        // If there's a valid target, set it as the new target
        if (validTargets.Count > 0)
        {
            currentTarget = validTargets[Random.Range(0, validTargets.Count)];
        }
        else
        {
            // If no more valid targets, destroy the projectile
            Destroy(gameObject);
        }
    }

    void DealDamageToEnemiesInPath(Vector2 direction)
    {
        // Check for enemies within a small distance ahead of the projectile
        float distanceToCheck = 0.1f; // Adjust this distance based on your projectile speed
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, distanceToCheck);

        foreach (RaycastHit2D hit in hits)
        {
            if ((hit.collider.CompareTag("Enemy") || hit.collider.CompareTag("Boss") || hit.collider.CompareTag("MiniBoss")) && !hitEnemies.Contains(hit.collider.gameObject))
            {
                Health enemyHealth = hit.collider.GetComponent<Health>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage);
                    hitEnemies.Add(hit.collider.gameObject);  // Mark this enemy as hit to avoid multiple hits
                    Debug.Log("Ricochet Hit enemy through path");
                }
            }
        }
    }

    void HitEnemy(GameObject enemy)
    {
        // Deal damage to the enemy
        Health enemyHealth = enemy.GetComponent<Health>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
            Debug.Log("Ricochet Hit enemy");
        }

        hitEnemies.Add(enemy);  // Add to the list of hit enemies
    }
}
