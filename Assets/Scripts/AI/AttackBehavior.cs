using UnityEngine;
using System.Collections;

public class AttackBehavior : BaseBehavior {

    /* *** Interface Methods *** */

    protected override void _Update() {
        NpcState myState = _controller.myState;

        // If we can see the player, then aim at him.
        Vector2 playerPosition = (Vector2)_controller.playerState.transform.position + _controller.estimatedPlayerVelocity;
        _controller.reticle.LerpTo(playerPosition, myState.lookSpeed);

        // Fire the equipped weapon
        var gun = myState.equippedFirearm;
        if (gun != null) {
            if (gun.IsEmpty) {
                gun.Reload();
            } else if (myState.canSeePlayer) {
                // If we can't see the player, then there's nothing to fire at.
                _controller.Fire();
            }
        }
    }

    protected override void _FixedUpdate() {
        _controller.character.velocity = Vector2.zero;
    }
}
