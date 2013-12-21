using UnityEngine;
using System.Collections;

public class AttackBehavior : INpcBehavior {

    protected NpcController _controller;

    public AttackBehavior(NpcController controller) {
        _controller = controller;
    }

    /* *** Interface Methods *** */

    public void doUpdate() {
        NpcState myState = _controller.myState;

        // If we can see the player, then aim at him.
        Vector3 playerPosition = _controller.playerState.transform.position + _controller.estimatedPlayerVelocity;
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

    public void doFixedUpdate() {
        _controller.character.velocity = Vector3.zero;
    }
}
