using UnityEngine;
using System.Collections;

public class PlayerCharacterController : BaseCharacterController {

    public bool isPlayerInputEnabled;
    private float _horizontalInput;
	private float _verticalInput;

    protected override void Act() {
        var velocity = new Vector3(_horizontalInput, _verticalInput);
        _character.velocity = Vector3.ClampMagnitude(velocity, 1) * _character.maxWalkSpeed;
        _moveable.velocity = new Vector3(_character.velocity.x, _character.velocity.y);
    }

    protected override void Aim() {
        var mouseDelta = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        _reticle.SetPosition(_reticle.transform.position + mouseDelta);
    }

    public void Update() {
		
        if (this.isPlayerInputEnabled) {
            _horizontalInput = Input.GetAxis("Horizontal"); // -1.0 to 1.0
            _verticalInput = Input.GetAxis("Vertical"); // -1.0 to 1.0

            Aim();

            if (equippedFirearm != null) {
                if (equippedFirearm.fullAuto && Input.GetButton("Fire1")) {
                    Fire();
                } else if (!equippedFirearm.fullAuto && Input.GetButtonDown("Fire1")) {
                    Fire();
                } else if (Input.GetButton("Reload")) {
                    equippedFirearm.Reload();
                }
            }

            if (Input.GetButtonDown("Next Weapon")) {
                _inventory.NextWeapon();
            } else if (Input.GetButtonDown("Previous Weapon")) {
                _inventory.PreviousWeapon();
            }
        }
    }
}
