using UnityEngine;
using System.Collections;

/// <summary>
/// Scene controller.
/// Code for managing the scene should be included here.
/// </summary>
public class SceneController : MonoBehaviour {

    /* *** Member Variables *** */

    public static bool isPaused = false;

    /* *** Constructors *** */

    void Awake() {
        // Hide the OS cursor
        Screen.lockCursor = true;
    }

    /* *** MonoBehaviour Methods *** */

    void Update() {
        if (Input.GetKeyDown("escape")) {
            // Pause the game and unlock the cursor
            Screen.lockCursor = false;
            isPaused = true;
            Time.timeScale = 0f; // Stop time - pause movement
        } else if (Input.GetMouseButtonDown(0)) {
            // Unpause when the screen receives focus again
            Screen.lockCursor = true;
            isPaused = false;
            Time.timeScale = 1f; // Restart time
        }
    }
}
