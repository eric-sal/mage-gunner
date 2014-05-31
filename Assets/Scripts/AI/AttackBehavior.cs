using UnityEngine;
using System.Collections;

public class AttackBehavior : BaseBehavior {

    /* *** Interface Methods *** */

    protected override void _Update() {
        NpcState myState = _controller.myState;

        // If we can see the player, then aim at him - with a little bit of a lead.
        Vector3 playerPosition = _controller.playerState.transform.position + (_controller.estimatedPlayerVelocity * Time.fixedDeltaTime * 10);
        _controller.reticle.SetPosition(playerPosition);

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
        _controller.character.velocity = Vector3.zero;
    }
}
