using UnityEngine;
using System.Collections;

public class PlayerCharacterController : AbstractCharacterController {

    public bool isPlayerInputEnabled;
    private float _horizontalInput;
	private float _verticalInput;

    protected override void Act() {
		
		_character.velocity.y = _character.maxWalkSpeed * _verticalInput;
        _character.velocity.x = _character.maxWalkSpeed * _horizontalInput;

        if (_horizontalInput > 0) {
            _character.facing = Vector2.right;
        } else if (_horizontalInput < 0) {
            _character.facing = -Vector2.right;
        }

        return;
    }

    public void Update() {
		
        if (this.isPlayerInputEnabled) {
            _horizontalInput = Input.GetAxis("Horizontal"); // -1.0 to 1.0
			_verticalInput = Input.GetAxis("Vertical"); // -1.0 to 1.0
        }
    }
}
