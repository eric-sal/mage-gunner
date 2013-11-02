using UnityEngine;
using System.Collections;

/// <summary>
/// Scene controller.
/// Put any scene-related static vars in this class.
/// </summary>
public class SceneController : MonoBehaviour {
    public static bool isPaused = false;

    void Awake() {
        Screen.lockCursor = true;
    }

    void Update() {
        if (Input.GetKeyDown("escape")) {
            Screen.lockCursor = false;
            isPaused = true;
            Time.timeScale = 0f; // Stop time - pause movement
        } else if (Input.GetMouseButtonDown(0)) {
            Screen.lockCursor = true;
            isPaused = false;
            Time.timeScale = 1f; // Restart time
        }
    }
}
