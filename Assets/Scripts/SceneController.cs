using UnityEngine;
using System.Collections;

/// <summary>
/// Scene controller.
/// Code for managing the scene should be included here.
/// </summary>
public class SceneController : MonoBehaviour {

    /* *** Member Variables *** */

    public static bool isPaused = false;
    private PauseMenu _pauseMenu;

    /* *** Constructors *** */

    void Awake() {
        // Hide the OS cursor
        Screen.lockCursor = true;
        _pauseMenu = GetComponent<PauseMenu>();
        _pauseMenu.enabled = false;
    }

    /* *** MonoBehaviour Methods *** */

    void Update() {
        if (Input.GetKeyDown("escape")) {
            // Pause the game and unlock the cursor
            Screen.lockCursor = !Screen.lockCursor;
            isPaused = !isPaused;
            _pauseMenu.enabled = !_pauseMenu.enabled;

            if (isPaused) {
                Time.timeScale = 0f; // Stop time - pause movement
            } else {
                Time.timeScale = 1f; // Restart time
            }
        }
    }
}
