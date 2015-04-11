using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Scene controller.
/// Code for managing the scene should be included here.
/// </summary>
public class SceneController : MonoBehaviour {

    /* *** Member Variables *** */

    public static bool isPaused = false;
    public static HashSet<CoverWaypoint> activeCoverWaypoints;
    private PauseMenu _pauseMenu;

    /* *** Constructors *** */

    void Awake() {
        Cursor.visible = false;
        _pauseMenu = GetComponent<PauseMenu>();
        _pauseMenu.enabled = false;
        activeCoverWaypoints = new HashSet<CoverWaypoint>();
    }

    /* *** MonoBehaviour Methods *** */

    void Update() {
        if (Input.GetKeyDown("escape")) {
            // Pause the game and unlock the cursor
            Cursor.visible = !Cursor.visible;
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
