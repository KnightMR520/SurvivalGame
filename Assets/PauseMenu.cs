using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Assign PauseMenuCanvas in the Inspector
    public Slider musicVolumeSlider;    // Reference to the volume slider
    public Slider soundEffectsVolumeSlider;    // Reference to the volume slider
    public AudioSource musicAudioSource; // Reference to the main audio source
    public AudioSource soundEffectsAudioSource; // Reference to the main audio source
    private bool isPaused = false;  // Track if the game is paused

    void Start()
    {

        // Ensure time scale is set to 1 when the game starts
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false); // Hide the menu initially
        musicVolumeSlider.onValueChanged.AddListener(ChangeMusicVolume); // Listen for slider changes
        soundEffectsVolumeSlider.onValueChanged.AddListener(ChangeSoundEffectsVolume); // Listen for slider changes

    }

    void Update()
    {
        // Check if the Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    // Pause the game
    public void Pause()
    {
        pauseMenuUI.SetActive(true);      // Show the pause menu
        Time.timeScale = 0f;              // Freeze the game
        isPaused = true;
    }

    // Resume the game
    public void Resume()
    {
        pauseMenuUI.SetActive(false);     // Hide the pause menu
        Time.timeScale = 1f;              // Unfreeze the game
        isPaused = false;
    }

    // Quit the game (For now, it just exits Play mode or closes the build)
    public void QuitGame()
    {
        SceneManager.LoadScene("HomeScreen");
        //Application.Quit(); // In Unity Editor, this won't close the editor but in the build, it will close the game
        //#if UNITY_EDITOR
        //UnityEditor.EditorApplication.isPlaying = false; // For exiting Play mode in the editor
        //#endif
    }

    // Change the master volume based on the slider value
    public void ChangeMusicVolume(float volume)
    {
        musicAudioSource.volume = volume; // Assuming you have a global audio source for the master volume
    }
    // Change the master volume based on the slider value
    public void ChangeSoundEffectsVolume(float volume)
    {
        soundEffectsAudioSource.volume = volume; // Assuming you have a global audio source for the master volume
    }
}
