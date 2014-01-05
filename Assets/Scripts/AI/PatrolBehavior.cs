﻿using UnityEngine;
using System.Collections;

public class PatrolBehavior : BaseBehavior {

    /* *** Interface Methods *** */

    protected override void _Update() {
        BaseCharacterState character = _controller.character;

        // If we can't see the player, aim in the direction we're moving.
        if (character.velocity != Vector2.zero) {
            // Because the velocity vector describes a unit vector from the origin, we have to
            // translate it to the NPC's current position.
            // NOTE: The 2 is an arbitrary scalar. Should that value be _myState.sightDistance?
            // Or can we keep it as a unit vector? How does the distance of the reticle from the
            // NPC affect the speed with which they aim at the player? My hunch is that by using
            // Vector2.Lerp, it makes no difference. If we were to move the the reticle by setting
            // a velocity though, the distance would make a difference.
            Vector2 velocity = character.velocity;
            Vector2 position = (Vector2)character.transform.position;
            _controller.reticle.LerpTo(position + velocity * 2, _controller.myState.lookSpeed);
        }
    }

    protected override void _FixedUpdate() {
        _controller.pathfinderAI.MoveAlongPath(_controller.myState.maxWalkSpeed);
    }
}
