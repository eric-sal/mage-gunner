using UnityEngine;
using System.Collections;

public class PlayerCharacterController : AbstractCharacterController {

    public bool isPlayerInputEnabled;
    private float _horizontalInput;
	private float _verticalInput;


    protected override void Act() {
        var velocity = new Vector2(_horizontalInput, _verticalInput) * _character.maxWalkSpeed;
        _character.velocity = Vector2.ClampMagnitude(velocity, _character.maxWalkSpeed);
        _moveable.velocity = new Vector3(_character.velocity.x, _character.velocity.y);
    }

    public void Update() {
		
        if (this.isPlayerInputEnabled) {
            _horizontalInput = Input.GetAxis("Horizontal"); // -1.0 to 1.0
			_verticalInput = Input.GetAxis("Vertical"); // -1.0 to 1.0

            if (Input.GetMouseButtonDown(0)) {

                // simulate weapon fire
                Debug.Log("BANG!");
                var recoil = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                _reticle.ApplyRecoil(recoil);
            }

        }
    }
}
