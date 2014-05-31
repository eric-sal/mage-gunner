using UnityEngine;
using System.Collections;

/// <summary>
/// Reticle controller.
/// </summary>
public class ReticleController : MonoBehaviour {

    /* *** Member Variables *** */

    public bool constrainToScreen = false;
    public bool willUpdateReticlePosition = false;
    public bool visible = false;

    private BaseCharacterState _character;
    private Vector3 _recoil;

    /// <summary>
    /// The position of the reticle offset by our recoil.
    /// It represents the position at which the next bullet will fire
    /// which isn't necessarily the same position where the player sees the reticle.
    /// This isn't ever actually drawn onscreen.
    /// </summary>
    public Vector3 ActualTargetPosition {
        get { return this.transform.position + _recoil * Time.fixedDeltaTime; }
    }

    /* *** Constructors *** */

    void Start() {
        _recoil = Vector3.zero;

        GetComponent<SpriteRenderer>().enabled = visible;

        _character = this.transform.parent.gameObject.GetComponent<BaseCharacterState>();
        if (_character == null) {
            string errMsg = "The game object for {0} requires a BaseCharacterState component!";
            throw new MissingComponentException(string.Format(errMsg, this.gameObject.name));
        }
    }

    /* *** MonoBehaviour Methods *** */

    void FixedUpdate() {
        // Reduce recoil before it moves the reticle.
        // This way, the reticle will not move for high strength
        // characters that are shooting low recoil weapons.
        ReduceRecoil(_character.recoilReductionRate);
        if (this.willUpdateReticlePosition) {
            SetPosition(this.ActualTargetPosition);
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(this.ActualTargetPosition, 0.5f);
    }

    /* *** Member Methods *** */

    /// <summary>
    /// Moves the reticle by adding the delta vector.
    /// </summary>
    /// <param name='delta'>
    /// The offset vector to move by.
    /// </param>
    public void MoveBy(Vector3 worldPositionDelta) {
        SetPosition(this.transform.position + worldPositionDelta);
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
    public void LerpTo(Vector3 worldPosition, float speed = 1) {
        if (SceneController.isPaused) {
            return;
        }

        Vector3 position = worldPosition;
        if (this.constrainToScreen) {
            position = CameraController.ConstrainPositionToScreen(worldPosition);
        }

        this.transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * speed);
    }

    /// <summary>
    /// Sets the position of the reticle on the screen.
    /// </summary>
    /// <param name='worldPosition'>
    /// The position of the player's cursor in world space.
    /// </param>
    public void SetPosition(Vector3 worldPosition) {
        if (SceneController.isPaused) {
            return;
        }

        Vector3 position = worldPosition;
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
    public void AddRecoil(Vector3 recoil) {
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
            _recoil = Vector3.zero;
        }
    }

    public void ResetRecoil() {
        _recoil = Vector3.zero;
    }
}
