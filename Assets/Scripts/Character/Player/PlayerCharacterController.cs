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
        float mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity");

        base.Update();

        // Aim: Move the player's reticle.
        var mouseDelta = new Vector2(Input.GetAxis("Mouse X") * mouseSensitivity, Input.GetAxis("Mouse Y") * mouseSensitivity);
        _reticle.MoveBy(mouseDelta);

        if (this.isPlayerInputEnabled) {

            if (_character.isDodging) {
                return;
            }

            // Get player input from the keyboard or joystick
            _horizontalInput = Input.GetAxis("Horizontal"); // -1.0 to 1.0
            _verticalInput = Input.GetAxis("Vertical"); // -1.0 to 1.0

            if (Input.GetButton("Jump")) {
                StartCoroutine(Dodge(0.5f));
            }

            // Fire the equipped weapon
//            Firearm gun = _character.equippedFirearm;
//            if (gun != null) {
//                if (gun.fullAuto && Input.GetButton("Fire1") ||
//                    !gun.fullAuto && Input.GetButtonDown("Fire1")) {
//                    // If full-auto is enabled and the player is holding down the fire button, or
//                    // if the firearm only allows for semi-auto fire, and the player clicks the fire button.
//                    _Fire();
//                } else if (Input.GetButton("Reload")) {
//                    gun.Reload();
//                }
//            }
            if (Input.GetButton("Fire1")) {
                var grenade = GameObject.Find("Grenade");
                if (grenade.rigidbody.velocity == Vector3.zero) {
                    var direction = _character.lookDirection.normalized * 8;
                    //grenade.rigidbody.AddForce();
                    var p = grenade.transform.position;
                    var position = new Vector3(p.x, p.y + 0.4f, p.z);
                    grenade.rigidbody.AddForceAtPosition(new Vector3(direction.x, direction.y, -30),
                                                         position);
                }
            }

            // Cycle through weapons
            if (Input.GetButtonDown("Next Weapon")) {
                EquipWeapon(_inventory.NextWeapon());
            } else if (Input.GetButtonDown("Previous Weapon")) {
                EquipWeapon(_inventory.PreviousWeapon());
            }
        }
    }

    /// <summary>
    /// Update the player's velocity.
    /// </summary>
    public override void FixedUpdate() {

        var velocity = new Vector3(_horizontalInput, _verticalInput);
        if (!_character.isDodging) {
            var maxVelocity = _character.maxWalkSpeed;
            _character.velocity = Vector3.ClampMagnitude(velocity * maxVelocity, maxVelocity);
            this.rigidbody.velocity = new Vector3(_character.velocity.x, _character.velocity.y);
        }
        base.FixedUpdate();
    }

    public IEnumerator Dodge(float duration) {
        _character.isDodging = true;
        this.isPlayerInputEnabled = false;
        _character.transform.rigidbody.AddForce(_character.velocity.normalized * 1000);
        _character.transform.rigidbody.drag = 10;
        yield return new WaitForSeconds(duration);
        _character.isDodging = false;
        this.isPlayerInputEnabled = true;
        _character.transform.rigidbody.drag = 0;
    }

}
