using UnityEngine;

public class GemSpawner : MonoBehaviour
{
    public GameObject gemPrefab1;  // The original gem (before 5 minutes)
    public GameObject gemPrefab2;  // The gem that spawns after 5 minutes
    public GameObject gemPrefab3;  // The gem that spawns after 10 minutes
    public GameObject gemPrefab4;  // The gem that spawns after 15 minutes

    private float elapsedTime = 0f;

    void Update()
    {
        elapsedTime += Time.deltaTime;  // Track elapsed game time
    }

    // Call this function when an enemy dies, or at the appropriate spawn point
    public void SpawnGem(Vector3 spawnPosition)
    {
        GameObject gemToSpawn = gemPrefab1;  // Default to gemPrefab1

        if (elapsedTime >= 15 * 60)  // 15 minutes
        {
            gemToSpawn = gemPrefab4;
        }
        else if (elapsedTime >= 10 * 60)  // 10 minutes
        {
            gemToSpawn = gemPrefab3;
        }
        else if (elapsedTime >= 5 * 60)  // 5 minutes
        {
            gemToSpawn = gemPrefab2;
        }

        Instantiate(gemToSpawn, spawnPosition, Quaternion.identity);  // Spawn the selected gem
    }
}
