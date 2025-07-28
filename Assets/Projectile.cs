using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float damage;  // Damage dealt by the projectile

    // Method to set the damage from outside this script
    public void SetDamage(float damageValue)
    {
        damage = damageValue;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Boss") || collision.CompareTag("MiniBoss"))
        {
            // Access the enemy's health script and deal damage
            Health enemyHealth = collision.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);  // Use the dynamically set damage
            }
            // Destroy the projectile after it hits an enemy
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Obstacle"))
        {
            // Destroy the bullet upon hitting an obstacle
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 2f);  // Destroy projectile after 2 seconds if it doesn't hit anything
    }

    // Update is called once per frame
    void Update()
    {
        // Ensure projectile stays on the correct Z position
        if (transform.position.z != 1)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 1);
        }
    }
}
