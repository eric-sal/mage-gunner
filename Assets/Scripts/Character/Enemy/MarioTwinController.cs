using UnityEngine;
using System.Collections;

public class MarioTwinController : AbstractCharacterController {

    public bool changeDirectionOnCollision;
    public bool jumpOnCollision;
    public bool changeDirectionAtLedge;
    public bool jumpAtLedge;

    protected override void Act() {
        _character.velocity.x = _character.maxWalkSpeed * _character.facing.x;
        if (_character.velocity.y < 0) {
            // we are falling or about to fall!
            Jump();
        }
    }

    protected override void OnLedgeReached(Vector3 direction) {
        if (direction.x == _character.facing.x) {
            if (changeDirectionAtLedge) {
                _character.facing.x *= -1;
            }
 
            if (jumpAtLedge) {
                Jump();
            }
        }
    }
}
