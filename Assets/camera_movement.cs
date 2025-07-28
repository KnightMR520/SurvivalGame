using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_movement : MonoBehaviour
{

    public Transform player;     // Reference to the player's transform
    public Vector3 offset;       // Offset to maintain a distance from the player

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Set the camera's position to follow the player with the given offset
        if (player != null)
        {
            transform.position = player.position + offset;
        }
    }
}
