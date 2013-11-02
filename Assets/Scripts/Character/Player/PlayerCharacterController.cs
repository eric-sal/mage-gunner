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

    protected override void Aim() {
        var mouseDelta = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        _reticle.SetPosition(_reticle.transform.position + mouseDelta);
    }

    public void Update() {
		
        if (this.isPlayerInputEnabled) {
            _horizontalInput = Input.GetAxis("Horizontal"); // -1.0 to 1.0
			_verticalInput = Input.GetAxis("Vertical"); // -1.0 to 1.0

            Aim();

            if (Input.GetMouseButton(0)) {

                // simulate weapon fire
                Debug.Log("BANG!");
                var recoil = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
                _reticle.ApplyRecoil(recoil);
            }

        }
    }
}
