using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;  // If using TextMeshPro
using UnityEngine.UI;  // If using regular UI Text

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText; // For TextMeshPro
    public float elapsedTime = 0f;

    void Update()
    {
        // Increment the elapsed time by the time passed since last frame
        elapsedTime += Time.deltaTime;

        // Format the elapsed time into minutes and seconds
        int minutes = Mathf.FloorToInt(elapsedTime / 60F);
        int seconds = Mathf.FloorToInt(elapsedTime % 60F);

        // Update the UI Text with the formatted time
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Method to set elapsed time for testing purposes
    public void SetElapsedTime(float newTime)
    {
        elapsedTime = newTime;
    }
}
