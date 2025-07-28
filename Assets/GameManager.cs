using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Include this to manage scenes


public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;              // Unfreeze the game
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Call this method to restart the game
    public void RestartGame()
    {
        // Reload the current scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }

    public void QuitGame()
    {
        // Reload the current scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("HomeScreen");
    }
}
