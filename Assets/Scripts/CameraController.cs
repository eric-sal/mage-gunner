using UnityEngine;
using System.Collections;

/// <summary>
/// Controls the main camera centered on the player.
/// </summary>
public class CameraController : MonoBehaviour {

    /* *** Member Variables *** */

    public Transform player; // set in unity

    private Camera _camera;

    /* *** Constructors *** */

    void Awake() {
        _camera = Camera.main;
    }

    /* *** MonoBehaviour Methods *** */

    void Update() {
        _camera.transform.position = new Vector3(player.position.x, _camera.transform.position.y, player.position.z + 7.5f);
    }

    /* *** Member Methods *** */

    /// <summary>
    /// Constrains the position to screen.
    /// </summary>
    /// <returns>
    /// A Vector3 in world space constrained to the limits of the screen.
    /// </returns>
    /// <param name='worldPosition'>
    /// A Vector3 representing a point in world space.
    /// </param>
    public static Vector3 ConstrainPositionToScreen(Vector3 worldPosition) {
        Camera camera = Camera.main;
        Vector3 screenPosition = camera.WorldToScreenPoint(worldPosition);

        if (screenPosition.x < 0f) {
            Vector3 leftmostPoint = new Vector3(0f, screenPosition.y, screenPosition.z);
            worldPosition = camera.ScreenToWorldPoint(leftmostPoint);
        } else if (screenPosition.x > camera.pixelWidth) {
            Vector3 rightmostPoint = new Vector3(camera.pixelWidth, screenPosition.y, screenPosition.z);
            worldPosition = camera.ScreenToWorldPoint(rightmostPoint);
        }

        if (screenPosition.y < 0f) {
            Vector3 bottommostPoint = new Vector3(screenPosition.x, 0f, screenPosition.z);
            worldPosition = camera.ScreenToWorldPoint(bottommostPoint);
        } else if (screenPosition.y > camera.pixelHeight) {
            Vector3 topmostPoint = new Vector3(screenPosition.x, camera.pixelHeight, screenPosition.z);
            worldPosition = camera.ScreenToWorldPoint(topmostPoint);
        }

        return worldPosition;
    }
}
