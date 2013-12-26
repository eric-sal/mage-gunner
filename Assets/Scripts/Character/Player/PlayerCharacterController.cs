using UnityEngine;
using System.Collections;

/// <summary>
/// Player character controller.
/// </summary>
public class PlayerCharacterController : BaseCharacterController {

    /* *** Member Variables *** */

    public bool isPlayerInputEnabled;

    private float _horizontalInput;
    private float _verticalInput;

    /* *** Constructors *** */

    public override void Awake() {
        base.Awake();
        _reticle.constrainToScreen = true;
        _reticle.visible = true;
    }

    /* *** MonoBehaviour Methods *** */

    /// <summary>
    /// Capture input from the player.
    /// </summary>
    public override void Update() {
        base.Update();

        if (this.isPlayerInputEnabled) {
            // Get player input from the keyboard or joystick
            _horizontalInput = Input.GetAxis("Horizontal"); // -1.0 to 1.0
            _verticalInput = Input.GetAxis("Vertical"); // -1.0 to 1.0

            var walking = _verticalInput != 0 || _horizontalInput != 0;
            _animator.SetBool("walking", walking);

            var localScale = _animator.transform.localScale;
            if (_horizontalInput < 0) {
                localScale.x = -1;
            } else {
                localScale.x = 1;
            }
            _animator.transform.localScale = localScale;

            _animator.SetFloat("inputX", Mathf.Abs(_horizontalInput));
            _animator.SetFloat("inputY", _verticalInput);

            // Aim: Move the player's reticle.
            var mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            _reticle.MoveBy(mouseDelta);

            // Fire the equipped weapon
            Firearm gun = _character.equippedFirearm;
            if (gun != null) {
                if (gun.fullAuto && Input.GetButton("Fire1") ||
                    !gun.fullAuto && Input.GetButtonDown("Fire1")) {
                    // If full-auto is enabled and the player is holding down the fire button, or
                    // if the firearm only allows for semi-auto fire, and the player clicks the fire button.
                    _Fire();
                } else if (Input.GetButton("Reload")) {
                    gun.Reload();
                }
            }

            // Cycle through weapons
            if (Input.GetButtonDown("Next Weapon")) {
                _inventory.NextWeapon();
            } else if (Input.GetButtonDown("Previous Weapon")) {
                _inventory.PreviousWeapon();
            }
        }
    }

    /// <summary>
    /// Update the player's velocity.
    /// </summary>
    public override void FixedUpdate() {
        var velocity = new Vector2(_horizontalInput, _verticalInput);
        _character.velocity = Vector2.ClampMagnitude(velocity, 1) * _character.maxWalkSpeed;

        base.FixedUpdate();
    }
}
