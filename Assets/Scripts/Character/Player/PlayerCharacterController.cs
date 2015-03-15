using UnityEngine;
using System.Collections;

/// <summary>
/// Player character controller.
/// </summary>
public class PlayerCharacterController : BaseCharacterController {

    /* *** Member Variables *** */

    public bool isPlayerInputEnabled;
    
    protected float _horizontalInput;
    protected float _verticalInput;

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

            if (!_character.isDodging) {
                // Get player input from the keyboard or joystick
                _horizontalInput = Input.GetAxis("Horizontal"); // -1.0 to 1.0
                _verticalInput = Input.GetAxis("Vertical"); // -1.0 to 1.0

                if (Input.GetButtonDown("Jump")) {
                    StartCoroutine(Dodge());
                }
            }

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
//            if (Input.GetButton("Fire1")) {
//                var grenade = GameObject.Find("Grenade");
//                if (grenade.rigidbody.velocity == Vector3.zero) {
//                    var direction = _character.lookDirection.normalized * 8;
//                    //grenade.rigidbody.AddForce();
//                    var p = grenade.transform.position;
//                    var position = new Vector3(p.x, p.y + 0.4f, p.z);
//                    grenade.rigidbody.AddForceAtPosition(new Vector3(direction.x, direction.y, -30),
//                                                         position);
//                }
//            }

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
        if (!_character.isDodging) {
            var velocity = new Vector3(_horizontalInput, _verticalInput);
            var maxVelocity = _character.maxWalkSpeed;
            _character.velocity = Vector3.ClampMagnitude(velocity * maxVelocity, maxVelocity);
        }

        _rigidbody.velocity = new Vector3(_character.velocity.x, _character.velocity.y);
        base.FixedUpdate();
    }

    public IEnumerator Dodge() {
        _horizontalInput = 0;
        _verticalInput = 0;

        _character.isDodging = true;

        // Dodge by reducing velocity
        _character.velocity *= 4;  // Initially, give our player an extra boost of speed
        float d = 0.25f;  // Amount of time we want the slide to take
        float c = 0.0f;  // Amount of time passed since we started the slide
        float t = 0.0f;  // c / d = 0..1
        float factor = 1.0f;  // The factor we will multiply our velocity by = 1..0

        while (_character.velocity.sqrMagnitude > 1f) {  // Stop the loop once our velocity almost reaches zero.

            yield return new WaitForFixedUpdate();

            if (_rigidbody.velocity.sqrMagnitude < 0.5f) {
                StopDodge();
                yield break;
            }

            c += Time.deltaTime;
            t = c / d;

            // Equations describing the rate at which we want the velocity to fall off to zero.
            if (t > 0.75) {
                // If we're near the end of our slide, smooth out our velocity fall off.
                factor = Mathf.Cos(Mathf.PI * t - 0.5f) / 2 + 0.5f;
            } else {
                // Keep the velocity near initial value until about halfway through the slide,
                // then drop off sharply.
                factor = Mathf.Cos(Mathf.Pow(t, 2) * Mathf.PI) / 2 + 0.5f;
            }
            // Debug.Log(factor);

            _character.velocity = _character.velocity * factor;
        }

        StopDodge();
    }

    private void StopDodge() {
        _character.velocity = Vector3.zero;
        _character.isDodging = false;
    }

    public override bool WillPopUp(Vector3 fromDirection) {
        if (fromDirection == Vector3.up) {
            return Input.GetAxis("Vertical") >= 0.5f;
        } else if (fromDirection == Vector3.up) {
            return Input.GetAxis("Vertical") <= -0.5f;
        } else if (fromDirection == Vector3.left) {
            return Input.GetAxis("Horizontal") <= -0.5f;
        } else if (fromDirection == Vector3.right) {
            return Input.GetAxis("Horizontal") >= 0.5f;
        }
        
        throw new System.ArgumentException("Unhandled direction: " + fromDirection.ToString());
    }
}
