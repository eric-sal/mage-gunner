using UnityEngine;
using System.Collections;

public class MarioTwinCollisionHandler : CharacterCollisionHandler {
    private MarioTwinController _controller;
 
    public override void Awake() {
        _controller = GetComponent<MarioTwinController>();
        base.Awake();
    }

    public override void HandleCollision(Collider collidedWith, Vector3 fromDirection, float distance) {
        base.HandleCollision(collidedWith, fromDirection, distance);
        
        // If the twin's forward progress is stopped (i.e.: by a pipe or block), jump over it
        if (fromDirection == Vector3.right && _character.facing == Vector2.right ||
            fromDirection == Vector3.left && _character.facing == -Vector2.right) {
            _controller.Jump();
        }
    }
 
    public override void HandleCollision(PlayerCollisionHandler player, Vector3 fromDirection, float distance) {
        base.HandleCollision(player.collider, fromDirection, distance);

        if (fromDirection.y <= 0) {
            // If the player hits his twin from the bottom, left, or right, then jump
            _controller.Jump();
        }
    }

    public override void HandleCollision(PickupCollisionHandler pickup, Vector3 fromDirection, float distance) {
        _character.coinCount++;
    }
}
