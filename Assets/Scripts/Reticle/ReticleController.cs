using UnityEngine;
using System.Collections;

/// <summary>
/// Reticle controller.
/// </summary>
public class ReticleController : MonoBehaviour {

    /* *** Member Variables *** */

    public bool constrainToScreen = false;

    private Vector2 _recoil;

    /* *** Constructors *** */

    void Start() {
        _recoil = Vector2.zero;
    }

    /* *** MonoBehaviour Methods *** */
	
    void FixedUpdate() {
        SetPosition((Vector2)this.transform.position + _recoil);
    }

    /* *** Member Methods *** */

    /// <summary>
    /// Moves the reticle by adding the delta vector.
    /// </summary>
    /// <param name='delta'>
    /// The offset vector to move by.
    /// </param>
    public void MoveBy(Vector2 worldPositionDelta) {
        SetPosition((Vector2)this.transform.position + worldPositionDelta);
    }

    /// <summary>
    /// Lerps the reticle from its current position to the position we pass at the speed we specify.
    /// </summary>
    /// <param name='worldPosition'>
    /// The position of the player's cursor in world space.
    /// </param>
    /// <param name='speed'>
    /// The speed at which to move the reticle.
    /// </param>
    public void LerpTo(Vector2 worldPosition, float speed = 1) {
        if (SceneController.isPaused) {
            return;
        }

        Vector2 position = worldPosition;
        if (this.constrainToScreen) {
            position = CameraController.ConstrainPositionToScreen(worldPosition);
        }

        this.transform.position = Vector2.Lerp(transform.position, position, Time.deltaTime * speed);
    }

    /// <summary>
    /// Sets the position of the reticle on the screen.
    /// </summary>
    /// <param name='worldPosition'>
    /// The position of the player's cursor in world space.
    /// </param>
    public void SetPosition(Vector2 worldPosition) {
        if (SceneController.isPaused) {
            return;
        }

        Vector2 position = worldPosition;
        if (this.constrainToScreen) {
            position = CameraController.ConstrainPositionToScreen(worldPosition);
        }

        this.transform.position = position;
    }

    /// <summary>
    /// Applies recoil.
    /// </summary>
    /// <param name='recoil'>
    /// The amount of recoil to apply.
    /// </param>
    public void ApplyRecoil(Vector2 recoil) {
        _recoil += recoil;
    }

    /// <summary>
    /// Reduces recoil.
    /// </summary>
    /// <param name='amount'>
    /// A magnitude for the amount to reduce the recoil by.
    /// </param>
    public void ReduceRecoil(float amount) {
        float magBefore = _recoil.sqrMagnitude;
        _recoil -= _recoil.normalized * amount;
        float magAfter = _recoil.sqrMagnitude;

        if (magAfter > magBefore) {
            // slid too far in the opposite direction
            _recoil = Vector2.zero;
        }
    }
}
