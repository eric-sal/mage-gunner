using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {
    public float mouseSensitivity = 0.025f;

    void Awake() {
        if (PlayerPrefs.HasKey("MouseSensitivity")) {
            mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity");
        }
    }

    void OnGUI() {
        GUI.Window(0, new Rect(Screen.width / 2 - 50, Screen.height / 2 - 45, 100, 100), Draw, "Preferences");
    }

    void Draw(int windowID) {
        mouseSensitivity = GUI.HorizontalSlider(new Rect(5, 30, 90, 30), mouseSensitivity, 0.001f, 0.1f);
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity);

        GUI.Label(new Rect(5, 70, 90, 30), Mathf.Floor(mouseSensitivity * 1000).ToString());
    }
}
