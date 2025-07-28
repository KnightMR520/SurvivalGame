using UnityEngine;
using UnityEngine.UI;

public class ButtonSoundEffect : MonoBehaviour
{
    public AudioSource audioSource;  // Reference to the AudioSource component
    public AudioClip clickSound;     // The audio clip for the click sound

    void Start()
    {
        // Get the Button component and add a listener for the onClick event
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(PlaySound);
        }
    }

    // Method to play the click sound
    // Method to play the click sound
    void PlaySound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);  // Play the sound once on click
            Debug.Log("Sound should be playing now!");
        }
        else
        {
            Debug.LogError("AudioSource or clickSound is not assigned properly!");
        }
    }

}
