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

    /* *** MonoBehaviour Methods *** */

    /// <summary>
    /// Capture input from the player.
    /// </summary>
    public override void Update() {
        if (this.isPlayerInputEnabled) {
            // Get player input from the keyboard or joystick
            _horizontalInput = Input.GetAxis("Horizontal"); // -1.0 to 1.0
            _verticalInput = Input.GetAxis("Vertical"); // -1.0 to 1.0

            _Aim();

            // Fire the equipped weapon
            if (equippedFirearm != null) {
                if (equippedFirearm.fullAuto && Input.GetButton("Fire1") ||
                    !equippedFirearm.fullAuto && Input.GetButtonDown("Fire1")) {
                    // If full-auto is enabled and the player is holding down the fire button, or
                    // if the firearm only allows for semi-auto fire, and the player clicks the fire button.
                    _Fire();
                } else if (Input.GetButton("Reload")) {
                    equippedFirearm.Reload();
                }
            }

            // Cycle through weapons
            if (Input.GetButtonDown("Next Weapon")) {
                _inventory.NextWeapon();
            } else if (Input.GetButtonDown("Previous Weapon")) {
                _inventory.PreviousWeapon();
            }
        }

        base.Update();
    }

    /// <summary>
    /// Update the player's velocity.
    /// </summary>
    public override void FixedUpdate() {
        var velocity = new Vector3(_horizontalInput, _verticalInput);
        _moveable.velocity = Vector3.ClampMagnitude(velocity, 1) * _character.maxWalkSpeed;

        base.FixedUpdate();
    }

    /* *** Member Methods *** */

    /// <summary>
    /// Move the player's reticle.
    /// </summary>
    protected override void _Aim() {
        var mouseDelta = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        _reticle.MoveBy(mouseDelta);

        base._Aim();
    }
}
